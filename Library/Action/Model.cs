using CodeM.Common.DbHelper;
using CodeM.Common.Orm.Dialect;
using CodeM.Common.Orm.Serialize;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace CodeM.Common.Orm
{
    public partial class Model : ISetValue, IGetValue, IPaging, ISort, ICommand
    {
        #region ISetValue
        ModelObject mSetValues;

        internal ModelObject Values
        {
            get
            {
                return mSetValues;
            }
        }

        public Model SetValue(string name, object value)
        {
            if (mSetValues == null)
            {
                mSetValues = ModelObject.New(this);
            }
            mSetValues.SetValue(name, value);
            return this;
        }

        public Model SetValues(ModelObject obj)
        {
            if (obj != null)
            {
                for (int i = 0; i < PropertyCount; i++)
                {
                    Property p = GetProperty(i);
                    if (obj.Has(p.Name))
                    {
                        SetValue(p.Name, obj[p.Name]);
                    }
                }
            }
            return this;
        }
        #endregion

        #region IGetValue
        List<string> mGetValues = new List<string>();

        internal List<string> ReturnValues
        {
            get
            {
                return mGetValues;
            }
        }

        public Model GetValue(params string[] names)
        {
            foreach (string name in names)
            {
                string compactName = name.Trim();
                if (mGetValues.IndexOf(compactName) < 0)
                {
                    if (!compactName.Contains("."))
                    {
                        Property p = GetProperty(compactName);
                        if (p == null)
                        {
                            throw new Exception(string.Concat("未找到属性：", compactName));
                        }
                    }
                    else
                    {
                        string[] typeItems = compactName.Split(".");
                        Model currM = this;
                        for (int i = 0; i < typeItems.Length; i++)
                        {
                            Property p = currM.GetProperty(typeItems[i]);
                            if (p == null)
                            {
                                throw new Exception(string.Concat("未找到属性：", compactName));
                            }

                            if (i < typeItems.Length - 1)
                            {
                                currM = ModelUtils.GetModel(p.TypeValue);
                                if (currM == null)
                                {
                                    throw new Exception(string.Concat("非法的Model引用：", p.TypeValue));
                                }
                            }
                        }
                    }

                    mGetValues.Add(compactName);
                }
            }
            return this;
        }
        #endregion

        #region IFilter

        private SubFilter mFilter = new SubFilter();

        internal SubFilter Where
        {
            get
            {
                return mFilter;
            }
        }

        public Model And(IFilter subCondition)
        {
            mFilter.And(subCondition);
            return this;
        }

        public Model Or(IFilter subCondition)
        {
            mFilter.Or(subCondition);
            return this;
        }

        public Model Equals(string name, object value)
        {
            mFilter.Equals(name, value);
            return this;
        }

        public Model NotEquals(string name, object value)
        {
            mFilter.NotEquals(name, value);
            return this;
        }

        public Model Gt(string name, object value)
        {
            mFilter.Gt(name, value);
            return this;
        }

        public Model Gte(string name, object value)
        {
            mFilter.Gte(name, value);
            return this;
        }

        public Model Lt(string name, object value)
        {
            mFilter.Lt(name, value);
            return this;
        }

        public Model Lte(string name, object value)
        {
            mFilter.Lte(name, value);
            return this;
        }

        public Model Like(string name, string value)
        {
            mFilter.Like(name, value);
            return this;
        }

        public Model NotLike(string name, string value)
        {
            mFilter.NotLike(name, value);
            return this;
        }

        public Model IsNull(string name)
        {
            mFilter.IsNull(name);
            return this;
        }

        public Model IsNotNull(string name)
        {
            mFilter.IsNotNull(name);
            return this;
        }

        public Model Between(string name, object value, object value2)
        {
            mFilter.Between(name, value, value2);
            return this;
        }

        #endregion

        #region IPaging

        private bool mUsePaging = false;
        private int mPageSize = 100;
        private int mPageIndex = 1;

        internal bool IsUsePaging 
        {
            get
            {
                return mUsePaging;
            }
        }

        internal int CurrPageSize 
        {
            get 
            {
                return mPageSize;
            }
        }

        internal int CurrPageIndex 
        {
            get
            {
                return mPageIndex;
            }
        }

        public Model PageSize(int size)
        {
            if (size < 1)
            {
                throw new Exception("分页大小至少为1。");
            }

            mPageSize = size;
            mUsePaging = true;
            return this;
        }

        public Model PageIndex(int index)
        {
            if (index < 1)
            {
                throw new Exception("最小页码从1开始。");
            }

            mPageIndex = index;
            mUsePaging = true;
            return this;
        }

        public Model Top(int num)
        {
            if (num < 1)
            {
                throw new Exception("最小返回数据数量至少为1。");
            }

            mPageSize = 1;
            mPageIndex = 1;
            mUsePaging = true;
            return this;
        }

        #endregion

        #region ISort
        private List<string> mSorts = new List<string>();
        private List<string> mForeignSortNames = new List<string>();

        public Model AscendingSort(string name)
        {
            if (!name.Contains("."))
            {
                Property p = GetProperty(name);
                if (p == null)
                {
                    throw new Exception(string.Concat("未找到属性：", name));
                }
                mSorts.Add(string.Concat(p.Owner.Table, ".", p.Field, " ASC"));
            }
            else
            {
                Model currM = this;
                string[] subNames = name.Split(".");
                for (int i = 0; i < subNames.Length; i++)
                {
                    Property subProp = currM.GetProperty(subNames[0]);
                    Model subM = ModelUtils.GetModel(subProp.TypeValue);
                    currM = subM;

                    if (i == subNames.Length - 2)
                    {
                        Property lastProp = subM.GetProperty(subNames[i + 1]);
                        mSorts.Add(string.Concat(subM.Table, ".", lastProp.Field, " ASC"));
                        break;
                    }
                }

                mForeignSortNames.Add(name);
            }
            return this;
        }

        public Model DescendingSort(string name)
        {
            if (!name.Contains("."))
            {
                Property p = GetProperty(name);
                if (p == null)
                {
                    throw new Exception(string.Concat("未找到属性：", name));
                }
                mSorts.Add(string.Concat(p.Owner.Table, ".", p.Field, " DESC"));
            }
            else
            {
                Model currM = this;
                string[] subNames = name.Split(".");
                for (int i = 0; i < subNames.Length; i++)
                {
                    Property subProp = currM.GetProperty(subNames[0]);
                    Model subM = ModelUtils.GetModel(subProp.TypeValue);
                    currM = subM;

                    if (i == subNames.Length - 2)
                    {
                        Property lastProp = subM.GetProperty(subNames[i + 1]);
                        mSorts.Add(string.Concat(subM.Table, ".", lastProp.Field, " DESC"));
                        break;
                    }
                }

                mForeignSortNames.Add(name);
            }
            return this;
        }

        internal List<string> ForeignSortNames 
        {
            get
            {
                return mForeignSortNames;
            }
        }

        internal string Sort
        {
            get
            {
                return string.Join(",", mSorts);
            }
        }
        #endregion

        private void Reset()
        {
            mSetValues = null;
            mGetValues.Clear();
            mFilter.Reset();
            mSorts.Clear();
            mForeignSortNames.Clear();

            mUsePaging = false;
            mPageSize = 100;
            mPageIndex = 1;
        }

        #region ICommand
        public bool CreateTable(bool replace = false)
        {
            try
            {
                StringBuilder sb = new StringBuilder(ToString());
                if (replace)
                {
                    sb.Insert(0, string.Concat("DROP TABLE IF EXISTS ", Table, ";"));
                }

                DbUtils.ExecuteNonQuery(Path.ToLower(), sb.ToString());

                string tableIndexSQL = ToString(true);
                if (!string.IsNullOrWhiteSpace(tableIndexSQL))
                {
                    DbUtils.ExecuteNonQuery(Path.ToLower(), tableIndexSQL);
                }
                return true;
            }
            catch
            {
                ;
            }
            return false;
        }

        public bool RemoveTable()
        {
            try
            {
                string sql = string.Concat("DROP TABLE IF EXISTS ", Table);
                DbUtils.ExecuteNonQuery(Path.ToLower(), sql);
                return true;
            }
            catch
            {
                ;
            }
            return false;
        }

        public bool TruncateTable()
        {
            try
            {
                string sql = string.Concat("TRUNCATE TABLE ", Table);
                if (!Features.IsSupportTruncate(this))
                {
                    sql = string.Concat("DELETE FROM ", Table);
                }
                DbUtils.ExecuteNonQuery(Path.ToLower(), sql);
                return true;
            }
            catch
            {
                ;
            }
            return false;
        }

        private bool _CheckPropertyType(Property p, object value)
        {
            Type type = p.Type;
            if (type == typeof(bool))
            {
                bool result;
                if (!bool.TryParse("" + value, out result))
                {
                    throw new Exception("无效的bool数据：" + value);
                }
            }
            else if (type == typeof(byte))
            {
                byte result;
                if (!byte.TryParse("" + value, out result))
                {
                    throw new Exception("无效的byte数据：" + value);
                }
            }
            else if (type == typeof(sbyte))
            {
                sbyte result;
                if (!sbyte.TryParse("" + value, out result))
                {
                    throw new Exception("无效的sbyte数据：" + value);
                }
            }
            else if (type == typeof(decimal))
            {
                decimal result;
                if (!decimal.TryParse("" + value, out result))
                {
                    throw new Exception("无效的decimal数据：" + value);
                }
            }
            else if (type == typeof(double))
            {
                double result;
                if (!double.TryParse("" + value, out result))
                {
                    throw new Exception("无效的double数据：" + value);
                }
            }
            else if (type == typeof(Int16))
            {
                Int16 result;
                if (!Int16.TryParse("" + value, out result))
                {
                    throw new Exception(p.Name + "属性值类型必须是Int16数据：" + value);
                }
            }
            else if (type == typeof(Int32))
            {
                Int32 result;
                if (!Int32.TryParse("" + value, out result))
                {
                    throw new Exception(p.Name + "属性值类型必须是Int32：" + value);
                }
            }
            else if (type == typeof(Int64))
            {
                Int64 result;
                if (!Int64.TryParse("" + value, out result))
                {
                    throw new Exception(p.Name + "属性值类型必须是Int64：" + value);
                }
            }
            else if (type == typeof(Single))
            {
                Single result;
                if (!Single.TryParse("" + value, out result))
                {
                    throw new Exception(p.Name + "属性值类型必须是Single：" + value);
                }
            }
            else if (type == typeof(UInt16))
            {
                UInt16 result;
                if (!UInt16.TryParse("" + value, out result))
                {
                    throw new Exception(p.Name + "属性值类型必须是UInt16：" + value);
                }
            }
            else if (type == typeof(UInt32))
            {
                UInt32 result;
                if (!UInt32.TryParse("" + value, out result))
                {
                    throw new Exception(p.Name + "属性值类型必须是UInt32：" + value);
                }
            }
            else if (type == typeof(UInt64))
            {
                UInt64 result;
                if (!UInt64.TryParse("" + value, out result))
                {
                    throw new Exception(p.Name + "属性值类型必须是UInt64：" + value);
                }
            }
            else if (type == typeof(DateTime))
            {
                DateTime result;
                if (!DateTime.TryParse("" + value, out result))
                {
                    throw new Exception(p.Name + "属性值类型必须是DateTime：" + value);
                }
            }

            return true;
        }

        private void _CheckModelConstraint()
        {
            object value;
            int count = PropertyCount;
            for (int i = 0; i < count; i++)
            {
                Property p = GetProperty(i);
                mSetValues.TryGetValue(p.Name, out value);

                if (p.IsNotNull && !p.AutoIncrement)
                {
                    if (!mSetValues.Has(p.Name) || 
                        value == null)
                    {
                        throw new Exception("属性值不允许为空：" + p.Name);
                    }
                }

                if (value != null)
                {
                    _CheckPropertyType(p, value);
                } 
            }
        }

        private void _CalcBeforeSaveProperties()
        {
            foreach (Property p in mBeforeSavePropeties)
            {
                object value = p.DoBeforeSaveProcessor(mSetValues);
                if (!Undefined.IsUndefinedValue(value))
                {
                    SetValue(p.Name, value);
                }
            }
        }

        public bool Save(bool validate = false)
        {
            try
            {
                if (validate)
                {
                    _CheckModelConstraint();
                }

                if (mSetValues == null)
                {
                    throw new Exception("没有任何要保存的内容，请通过SetValue设置内容。");
                }

                _CalcBeforeSaveProperties();

                CommandSQL cmd = SQLBuilder.BuildInsertSQL(this);
                bool ret = DbUtils.ExecuteNonQuery(Path, cmd.SQL, cmd.Params.ToArray()) == 1;
                return ret;
            }
            finally
            {
                Reset();
            }
        }

        public bool Update(bool updateAll = false)
        {
            try
            {
                if (mSetValues == null)
                {
                    throw new Exception("没有任何要更新的内容，请通过SetValue设置内容。");
                }

                if (mFilter.IsEmpty() && !updateAll)
                {
                    throw new Exception("未设置更新的条件范围。");
                }

                _CalcBeforeSaveProperties();

                CommandSQL cmd = SQLBuilder.BuildUpdateSQL(this);
                bool ret = DbUtils.ExecuteNonQuery(Path, cmd.SQL, cmd.Params.ToArray()) > 0;

                return ret;
            }
            finally
            {
                Reset();
            }
        }

        public bool Delete(bool deleteAll = false)
        {
            try
            {
                if (mFilter.IsEmpty() && !deleteAll)
                {
                    throw new Exception("未设置删除的条件范围。");
                }

                CommandSQL where = mFilter.Build(this);

                string sql = string.Concat("DElETE FROM ", this.Table);
                if (!string.IsNullOrWhiteSpace(where.SQL))
                {
                    sql += string.Concat(" WHERE ", where.SQL);
                }

                bool ret = DbUtils.ExecuteNonQuery(this.Path, sql, where.Params.ToArray()) > 0;
                return ret;
            }
            finally
            {
                Reset();
            }
        }

        public List<dynamic> Query()
        {
            DbDataReader dr = null;
            try
            {
                if (mGetValues.Count == 0)
                {
                    for (int i = 0; i < PropertyCount; i++)
                    {
                        Property p = GetProperty(i);
                        mGetValues.Add(p.Name);
                    }
                }

                List<dynamic> result = new List<dynamic>();

                CommandSQL cmd = SQLBuilder.BuildQuerySQL(this);
                dr = DbUtils.ExecuteDataReader(Path, cmd.SQL, cmd.Params.ToArray());
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        ModelObject obj = ModelObject.New(this);
                        foreach (string name in mGetValues)
                        {
                            if (!name.Contains("."))
                            {
                                Property p = GetProperty(name);
                                if (dr.IsDBNull(name))
                                {
                                    obj.SetValue(name, null);
                                }
                                else
                                {
                                    obj.SetValue(name, dr.GetValue(name));
                                }

                                if (!string.IsNullOrWhiteSpace(p.AfterQueryProcessor))
                                {
                                    object value = Processor.Call(p.AfterQueryProcessor, this, name, obj);
                                    if (!Undefined.IsUndefinedValue(value))
                                    {
                                        obj.SetValue(name, value);
                                    }
                                }
                            }
                            else
                            {
                                Model currM = this;
                                ModelObject currObj = obj;
                                string[] subNames = name.Split(".");
                                for (int i = 0; i < subNames.Length; i++)
                                {
                                    string subName = subNames[i];
                                    Property subProp = currM.GetProperty(subName);
                                    Model subM = ModelUtils.GetModel(subProp.TypeValue);
                                    ModelObject subObj = ModelObject.New(subM);
                                    currObj.SetValue(subName, subObj);
                                    currM = subM;
                                    currObj = subObj;

                                    if (i == subNames.Length - 2)
                                    {
                                        string lastName = subNames[subNames.Length - 1];
                                        Property lastProp = currM.GetProperty(lastName);

                                        string fieldName = name.Replace(".", "_");
                                        if (dr.IsDBNull(fieldName))
                                        {
                                            currObj.SetValue(lastName, null);
                                        }
                                        else
                                        {
                                            currObj.SetValue(lastName, dr.GetValue(fieldName));
                                        }

                                        if (!string.IsNullOrWhiteSpace(lastProp.AfterQueryProcessor))
                                        {
                                            object value = Processor.Call(lastProp.AfterQueryProcessor, currM, lastName, currObj);
                                            if (!Undefined.IsUndefinedValue(value))
                                            {
                                                currObj.SetValue(lastName, value);
                                            }
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                        result.Add(obj);
                    }
                }

                return result;
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                }

                Reset();
            }
        }

        public long Count()
        {
            try
            {
                CommandSQL where = mFilter.Build(this);

                string sql = string.Concat("SELECT COUNT(1) FROM ", this.Table);
                if (!string.IsNullOrWhiteSpace(where.SQL))
                {
                    sql += string.Concat(" WHERE ", where.SQL);
                }

                object count = DbUtils.ExecuteScalar(this.Path, sql, where.Params.ToArray());
                return (long)count;
            }
            finally
            {
                Reset();
            }
        }

        public bool Exists()
        {
            DbDataReader dr = null;
            try
            {
                if (mFilter.IsEmpty())
                {
                    throw new Exception("未设置判断的条件范围。");
                }

                CommandSQL where = mFilter.Build(this);

                string sql = string.Concat("SELECT * FROM ", this.Table);
                if (!string.IsNullOrWhiteSpace(where.SQL))
                {
                    sql += string.Concat(" WHERE ", where.SQL);
                }
                sql += "LIMIT 0,1";

                dr = DbUtils.ExecuteDataReader(this.Path, sql, where.Params.ToArray());
                return dr.HasRows;
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                }
                Reset();
            }
        }
        #endregion
    }
}
