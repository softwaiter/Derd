﻿using CodeM.Common.Orm.SQL.Dialect;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CodeM.Common.Orm
{
    [Serializable]
    public partial class Model : ICloneable
    {
        private ConcurrentDictionary<string, Property> mProperties = new ConcurrentDictionary<string, Property>();
        private ConcurrentDictionary<string, Property> mPropertyFields = new ConcurrentDictionary<string, Property>();
        private ConcurrentDictionary<int, string> mPropertyIndexes = new ConcurrentDictionary<int, string>();

        private ConcurrentDictionary<string, Property> mPrimaryKeys = new ConcurrentDictionary<string, Property>();
        private ConcurrentDictionary<int, string> mPrimaryKeyIndexes = new ConcurrentDictionary<int, string>();

        private ConcurrentDictionary<string, string> mUniqueConstraints = new ConcurrentDictionary<string, string>();
        private ConcurrentDictionary<string, string> mIndexSettings = new ConcurrentDictionary<string, string>();

        private List<Property> mPreSavePropeties = new List<Property>();

        public string Path { get; set; }

        public string Name { get; set; }

        public string Table { get; set; }

        internal string BeforeNewProcessor { get; set; } = null;

        internal string AfterNewProcessor { get; set; } = null;

        internal string BeforeUpdateProcessor { get; set; } = null;

        internal string AfterUpdateProcessor { get; set; } = null;

        internal string BeforeDeleteProcessor { get; set; } = null;

        internal string AfterDeleteProcessor { get; set; } = null;

        internal string BeforeQueryProcessor { get; set; } = null;

        internal string AfterQueryProcessor { get; set; } = null;

        internal bool AddProperty(Property p)
        {
            p.Owner = this;

            if (mProperties.TryAdd(p.Name.ToLower(), p))
            {
                mPropertyFields.AddOrUpdate(p.Field.ToLower(), p, (key, value) =>
                {
                    return p;
                });

                mPropertyIndexes.AddOrUpdate(mPropertyIndexes.Count, p.Name, (key, value) => {
                    return p.Name;
                });

                if (p.IsPrimaryKey)
                {
                    mPrimaryKeys.AddOrUpdate(p.Name.ToLower(), p, (key, value) =>
                    {
                        return p;
                    });

                    mPrimaryKeyIndexes.AddOrUpdate(mPrimaryKeyIndexes.Count, p.Name, (key, value) =>
                    {
                        return p.Name;
                    });
                }

                if (!string.IsNullOrWhiteSpace(p.UniqueGroup))
                {
                    mUniqueConstraints.AddOrUpdate(p.UniqueGroup, p.Field, (key, value) =>
                    {
                        return string.Concat(value, ",", p.Field);
                    });
                }

                if (!string.IsNullOrWhiteSpace(p.IndexGroup))
                {
                    mIndexSettings.AddOrUpdate(p.IndexGroup, p.Field, (key, value) =>
                    {
                        return string.Concat(value, ",", p.Field);
                    });
                }

                if (!string.IsNullOrWhiteSpace(p.PreSaveProcessor))
                {
                    mPreSavePropeties.Add(p);
                }

                return true;
            }

            return false;
        }

        internal string GetPrimaryFields()
        {
            string result = null;
            if (mPrimaryKeys.Count > 0)
            {
                result = string.Join(',', mPrimaryKeys.Keys);
            }
            return result;
        }

        internal string GetUniqueGroupFields(string uniqueGroup)
        {
            string result = null;
            mUniqueConstraints.TryGetValue(uniqueGroup, out result);
            return result;
        }

        internal string GetIndexGroupFields(string indexGroup)
        {
            string result = null;
            mIndexSettings.TryGetValue(indexGroup, out result);
            return result;
        }

        public bool HasProperty(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (!name.Contains("."))
                {
                    return mProperties.ContainsKey(name.ToLower());
                }
                else
                {
                    Model currM = this;
                    string[] typeItems = name.Split(".");
                    for (int i = 0; i < typeItems.Length; i++)
                    {
                        if (currM.HasProperty(typeItems[i]))
                        {
                            if (i < typeItems.Length - 1)
                            {
                                Property p = currM.GetProperty(typeItems[i]);
                                currM = ModelUtils.GetModel(p.TypeValue);
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            return false;
        }

        public Property GetProperty(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new NullReferenceException(name);
            }

            Property result;
            if (!name.Contains("."))
            {
                if (mProperties.TryGetValue(name.Trim().ToLower(), out result))
                {
                    return result;
                }
            }
            else
            {
                int firstIndex = name.IndexOf(".");
                if (mProperties.TryGetValue(name.Substring(0, firstIndex).Trim().ToLower(), out result))
                {
                    Model currM = ModelUtils.GetModel(result.TypeValue);
                    return currM.GetProperty(name.Substring(firstIndex + 1));
                }
            }

            throw new Exception(string.Concat("属性未定义：", name));
        }

        public Property GetPropertyByField(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new NullReferenceException(field);
            }

            Property result;
            if (mPropertyFields.TryGetValue(field.Trim().ToLower(), out result))
            {
                return result;
            }

            throw new Exception(string.Concat("属性未定义：", field));
        }

        public Property GetProperty(int index)
        {
            if (mPropertyIndexes.ContainsKey(index))
            {
                string name = mPropertyIndexes[index];
                return GetProperty(name);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public int PropertyCount
        {
            get
            {
                return mProperties.Count;
            }
        }

        public Property GetPrimaryKey(int index)
        {
            if (mPrimaryKeyIndexes.ContainsKey(index))
            {
                string name = mPrimaryKeyIndexes[index];
                return GetProperty(name);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public int PrimaryKeyCount
        {
            get
            {
                return mPrimaryKeys.Count;
            }
        }

        private string BuildCommentSQL()
        {
            StringBuilder sb = new StringBuilder(PropertyCount * 10);
            for (int i = 0; i < PropertyCount; i++)
            {
                Property p = GetProperty(i);
                if (Features.IsSupportComment(this) &&
                    !string.IsNullOrWhiteSpace(p.Description))
                { 
                    string extCmd = Features.GetCommentExtCommand(this, Table, p.Field, p.Description);
                    if (!string.IsNullOrWhiteSpace(extCmd))
                    {
                        sb.Append(string.Concat(extCmd, ";"));
                    }
                }
            }
            return sb.ToString();
        }

        public string BuildCreateTableSQL()
        {
            string[] quotes = Features.GetObjectQuotes(this);

            StringBuilder sb = new StringBuilder(PropertyCount * 10);
            sb.Append(string.Concat("CREATE TABLE ", quotes[0], Table, quotes[1], "("));
            for (int i = 0; i < PropertyCount; i++)
            {
                if (i > 0)
                {
                    sb.Append(",");
                }
                Property p = GetProperty(i);
                sb.Append(p.ToString());
            }

            if (mUniqueConstraints.Count > 0)
            {
                int i = 0;
                IEnumerator<KeyValuePair<string, string>> e = mUniqueConstraints.GetEnumerator();
                while (e.MoveNext())
                {
                    if (PropertyCount > 0 || i > 0)
                    {
                        sb.Append(",");
                    }

                    string identifierArgs = string.Concat(Table, ",", e.Current.Value);
                    string name = string.Concat("UNQ$", CommandUtils.GetObjectIdentifier(identifierArgs.Split(",")));
                    if (name.Length > 30)
                    {
                        name = name.Substring(0, 30);
                    }
                    string constraintFields = e.Current.Value.Replace(",", string.Concat(quotes[1], ",", quotes[0]));
                    sb.Append(string.Concat("CONSTRAINT ", name, " UNIQUE (", quotes[0], constraintFields, quotes[1], ")"));

                    i++;
                }
            }
            sb.Append(")");
            if (Features.IsNeedCreateSuffix(this))
            {
                ConnectionSetting cs = ConnectionUtils.GetConnectionByModel(this);
                string extCreateSuffix = Features.GetCreateSuffixExtCommand(this, cs.Database);
                sb.Append(extCreateSuffix);
            }
            sb.Append(";");

            string extCmd = BuildCommentSQL();
            sb.Append(extCmd);

            return sb.ToString();
        }

        public string ToString(bool buildIndexSQL = false)
        {
            if (buildIndexSQL)
            {
                if (mIndexSettings.Count > 0)
                {
                    string[] quotes = Features.GetObjectQuotes(this);

                    StringBuilder sb = new StringBuilder((Table.Length + 30) * mIndexSettings.Count);
                    IEnumerator<KeyValuePair<string, string>> e = mIndexSettings.GetEnumerator();
                    while (e.MoveNext())
                    {
                        string identifierArgs = string.Concat(Table, ",", e.Current.Value);
                        string name = string.Concat("IDX$", CommandUtils.GetObjectIdentifier(identifierArgs.Split(",")));
                        if (name.Length > 30)
                        {
                            name = name.Substring(0, 30);
                        }
                        string indexFields = e.Current.Value.Replace(",", string.Concat(quotes[1], ",", quotes[0]));
                        sb.Append(string.Concat("CREATE INDEX ", name, " ON ", quotes[0], Table, quotes[1], "(", quotes[0], indexFields, quotes[1], ");"));
                    }
                    return sb.ToString();
                }
                return string.Empty;
            }
            return BuildCreateTableSQL();
        }

        public object Clone()
        {
            Model m = new Model();
            m.Path = this.Path;
            m.Name = this.Name;
            m.Table = this.Table;
            m.BeforeNewProcessor = this.BeforeNewProcessor;
            m.AfterNewProcessor = this.AfterNewProcessor;
            m.BeforeUpdateProcessor = this.BeforeUpdateProcessor;
            m.AfterUpdateProcessor = this.AfterUpdateProcessor;
            m.BeforeDeleteProcessor = this.BeforeDeleteProcessor;
            m.AfterDeleteProcessor = this.AfterDeleteProcessor;
            m.BeforeQueryProcessor = this.BeforeQueryProcessor;
            m.AfterQueryProcessor = this.AfterQueryProcessor;

            for (int i = 0; i < PropertyCount; i++)
            {
                Property newP = (Property)GetProperty(i).Clone();
                newP.Owner = m;
                m.AddProperty(newP);
            }
            
            m.Reset();

            return m;
        }
    }
}
