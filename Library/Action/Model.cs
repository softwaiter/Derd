using CodeM.Common.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace CodeM.Common.Orm
{
    public partial class Model : ISetValue, IGetValue, IPaging, ICommand
    {
        #region ISetValue
        ModelObject mSetValues;

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

        private SQLExecuteObj BuildInsertSQL()
        {
            SQLExecuteObj result = new SQLExecuteObj(SQLCommandType.Insert);
            result.Values = new List<DbParameter>();

            object value;
            string insertFields = string.Empty;
            string insertValues = string.Empty;
            for (int i = 0; i < PropertyCount; i++)
            {
                Property p = GetProperty(i);
                if (p.JoinInsert)
                {
                    if (mSetValues.TryGetValue(p.Name, out value))
                    {
                        DbParameter dp = DbUtils.CreateParam(Path, Guid.NewGuid().ToString("N"),
                            value, p.FieldType, ParameterDirection.Input);
                        result.Values.Add(dp);

                        if (insertFields.Length > 0)
                        {
                            insertFields += ",";
                        }
                        insertFields += p.Field;

                        if (insertValues.Length > 0)
                        {
                            insertValues += ",";
                        }
                        insertValues += "?";
                    }
                }
            }
            result.Command = string.Concat("INSERT INTO ", Table, " (", insertFields, ") VALUES(", insertValues + ")");

            return result;
        }

        public bool Save()
        {
            try
            {
                if (mSetValues == null)
                {
                    throw new Exception("没有任何要保存的内容，请通过SetValue设置内容。");
                }

                SQLExecuteObj execObj = BuildInsertSQL();
                bool ret = DbUtils.ExecuteNonQuery(Path, execObj.Command, execObj.Values.ToArray()) == 1;
                return ret;
            }
            finally
            {
                Reset();
            }
        }

        private SQLExecuteObj BuildUpdateSQL()
        {
            SQLExecuteObj execObj = new SQLExecuteObj(SQLCommandType.Update);
            execObj.Values = new List<DbParameter>();

            object value;
            string updateContent = string.Empty;
            for (int i = 0; i < PropertyCount; i++)
            {
                Property p = GetProperty(i);
                if (p.JoinUpdate)
                {
                    if (mSetValues.TryGetValue(p.Name, out value))
                    {
                        DbParameter dp = DbUtils.CreateParam(Path, Guid.NewGuid().ToString("N"),
                            value, p.FieldType, ParameterDirection.Input);
                        execObj.Values.Add(dp);

                        if (updateContent.Length > 0)
                        {
                            updateContent += ",";
                        }
                        updateContent += string.Concat(p.Field, "=?");
                    }
                }
            }
            execObj.Command = string.Concat("UPDATE ", Table, " SET ", updateContent);

            return execObj;
        }

        public bool Update(bool updateAll = false)
        {
            try
            {
                if (mSetValues == null)
                {
                    throw new Exception("没有任何要更新的内容，请通过SetValue设置内容。");
                }

                SQLExecuteObj execObj = BuildUpdateSQL();
                CommandSQL actionSQL = mCondition.Build(this);

                if (string.IsNullOrEmpty(actionSQL.SQL) && !updateAll)
                {
                    throw new Exception("未设置更新的条件范围。");
                }

                if (!string.IsNullOrEmpty(actionSQL.SQL))
                {
                    execObj.Command += string.Concat(" WHERE ", actionSQL.SQL);
                    execObj.Values.AddRange(actionSQL.Params);
                }

                bool ret = DbUtils.ExecuteNonQuery(Path, execObj.Command, execObj.Values.ToArray()) > 0;
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
                CommandSQL actionSQL = mCondition.Build(this);

                if (string.IsNullOrEmpty(actionSQL.SQL) && !deleteAll)
                {
                    throw new Exception("未设置删除的条件范围。");
                }

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

        private SQLExecuteObj BuildQuerySQL()
        {
            //TODO Join

            SQLExecuteObj execObj = new SQLExecuteObj(SQLCommandType.Update);

            StringBuilder sb = new StringBuilder();
            if (mGetValues.Count > 0)
            {
                foreach (string name in mGetValues)
                {
                    Property p = GetProperty(name);
                    if (sb.Length > 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append(string.Concat(p.Field, " AS ", name));
                }
            }
            else
            {
                sb.Append("*");
            }
            execObj.Command = string.Concat("SELECT ", sb, " FROM ", this.Table);

            return execObj;
        }

        public List<ModelObject> Query()
        {
            DbDataReader dr = null;
            try
            {
                List<ModelObject> result = new List<ModelObject>();

                SQLExecuteObj execObj = BuildQuerySQL();
                CommandSQL actionSQL = mCondition.Build(this);

                if (!string.IsNullOrEmpty(actionSQL.SQL))
                {
                    execObj.Command += string.Concat(" WHERE ", actionSQL.SQL);
                }

                if (mUsePaging)
                {
                    execObj.Command += string.Concat(" LIMIT ", (mPageIndex - 1) * mPageSize, ",", mPageSize);
                }

                dr = DbUtils.ExecuteDataReader(Path, execObj.Command, actionSQL.Params.ToArray());
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
