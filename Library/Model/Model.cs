using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CodeM.Common.Orm
{
    public partial class Model : ICloneable
    {
        private ConcurrentDictionary<string, Property> mProperties = new ConcurrentDictionary<string, Property>();
        private ConcurrentDictionary<string, Property> mPropertyFields = new ConcurrentDictionary<string, Property>();
        private ConcurrentDictionary<int, string> mPropertyIndexes = new ConcurrentDictionary<int, string>();

        private ConcurrentDictionary<string, Property> mPrimaryKeys = new ConcurrentDictionary<string, Property>();
        private ConcurrentDictionary<int, string> mPrimaryKeyIndexes = new ConcurrentDictionary<int, string>();

        private ConcurrentDictionary<string, string> mUniqueConstraints = new ConcurrentDictionary<string, string>();

        public string Path { get; set; }

        public string Name { get; set; }

        public string Table { get; set; }

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

        public Property GetProperty(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new NullReferenceException(name);
            }

            Property result;
            if (mProperties.TryGetValue(name.ToLower(), out result))
            {
                return result;
            }

            throw new Exception(string.Concat("未找到Property：", name));
        }

        public Property GetPropertyByField(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new NullReferenceException(field);
            }

            Property result;
            if (mPropertyFields.TryGetValue(field.ToLower(), out result))
            {
                return result;
            }

            throw new Exception(string.Concat("未找到Property：", field));
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

        public dynamic NewObject()
        {
            return ModelObject.New(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(PropertyCount * 10);
            sb.Append(string.Concat("CREATE TABLE ", Table, "("));
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
                    sb.Append(string.Concat("CONSTRAINT ", e.Current.Key, " UNIQUE (", e.Current.Value, ")"));
                    i++;
                }
            }

            sb.Append(");");
            return sb.ToString();
        }

        public object Clone()
        {
            Model m = new Model();
            m.Path = this.Path;
            m.Name = this.Name;
            m.Table = this.Table;

            IEnumerator<KeyValuePair<string, Property>> e = this.mProperties.GetEnumerator();
            while (e.MoveNext())
            {
                Property newP = (Property)e.Current.Value.Clone();
                newP.Owner = m;
                m.AddProperty(newP);
            }
            
            m.Reset();

            return m;
        }
    }
}
