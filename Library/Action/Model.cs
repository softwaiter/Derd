﻿using CodeM.Common.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace CodeM.Common.Orm
{
    public partial class Model : ISetValue, IGetValue, IPaging, ICommand
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
            mSetValues.TrySetValue(name, value);
            return this;
        }

        public Model SetValues(ModelObject obj)
        {
            if (obj != null)
            {
                object value;
                for (int i = 0; i < PropertyCount; i++)
                {
                    Property p = GetProperty(i);
                    if (obj.TryGetValue(p.Name, out value))
                    {
                        SetValue(p.Name, value);
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
                if (mGetValues.IndexOf(name) < 0)
                {
                    Property p = GetProperty(name);
                    if (p == null)
                    {
                        throw new Exception(string.Concat("未找到属性：", name));
                    }

                    mGetValues.Add(name);
                }
            }
            return this;
        }
        #endregion

        #region ICondition

        private SubCondition mCondition = new SubCondition();

        internal SubCondition Where
        {
            get
            {
                return mCondition;
            }
        }

        public Model And(ICondition subCondition)
        {
            mCondition.And(subCondition);
            return this;
        }

        public Model Or(ICondition subCondition)
        {
            mCondition.Or(subCondition);
            return this;
        }

        public Model Equals(string name, object value)
        {
            mCondition.Equals(name, value);
            return this;
        }

        public Model NotEquals(string name, object value)
        {
            mCondition.NotEquals(name, value);
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

        #endregion

        private void Reset()
        {
            mSetValues = null;
            mGetValues.Clear();
            mCondition.Reset();

            mUsePaging = false;
            mPageSize = 100;
            mPageIndex = 1;
        }

        public bool Save()
        {
            try
            {
                if (mSetValues == null)
                {
                    throw new Exception("没有任何要保存的内容，请通过SetValue设置内容。");
                }

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

                if (mCondition.IsEmpty() && !updateAll)
                {
                    throw new Exception("未设置更新的条件范围。");
                }

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
                if (mCondition.IsEmpty() && !deleteAll)
                {
                    throw new Exception("未设置删除的条件范围。");
                }

                CommandSQL actionSQL = mCondition.Build(this);

                string sql = string.Concat("DElETE FROM ", this.Table);
                if (!string.IsNullOrWhiteSpace(actionSQL.SQL))
                {
                    sql += string.Concat(" WHERE ", actionSQL.SQL);
                }

                bool ret = DbUtils.ExecuteNonQuery(this.Path, sql, actionSQL.Params.ToArray()) > 0;
                return ret;
            }
            finally
            {
                Reset();
            }
        }

        public List<ModelObject> Query()
        {
            DbDataReader dr = null;
            try
            {
                List<ModelObject> result = new List<ModelObject>();

                CommandSQL cmd = SQLBuilder.BuildQuerySQL(this);
                dr = DbUtils.ExecuteDataReader(Path, cmd.SQL, cmd.Params.ToArray());
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        ModelObject obj = ModelObject.New(this);
                        foreach (string name in mGetValues)
                        {
                            Property p = GetProperty(name);
                            if (dr.IsDBNull(name))
                            {
                                obj.TrySetValue(name, null);
                            }
                            else if (p.Type == typeof(string))
                            {
                                obj.TrySetValue(name, dr.GetString(name));
                            }
                            else if (p.Type == typeof(UInt16))
                            {
                                obj.TrySetValue(name, dr.GetInt32(name));
                            }
                            else if (p.Type == typeof(int))
                            {
                                obj.TrySetValue(name, dr.GetInt32(name));
                            }
                            else if (p.Type == typeof(long))
                            {
                                obj.TrySetValue(name, dr.GetInt64(name));
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
                CommandSQL actionSQL = mCondition.Build(this);

                string sql = string.Concat("SELECT COUNT(1) FROM ", this.Table);
                if (!string.IsNullOrWhiteSpace(actionSQL.SQL))
                {
                    sql += string.Concat(" WHERE ", actionSQL.SQL);
                }

                object count = DbUtils.ExecuteScalar(this.Path, sql, actionSQL.Params.ToArray());
                return (long)count;
            }
            finally
            {
                Reset();
            }
        }
    }
}
