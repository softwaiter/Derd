using CodeM.Common.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace CodeM.Common.Orm
{
    public partial class Model : ISetValue, ICommand
    {
        #region ISetValue
        ModelObject mValues;

        public Model SetValue(string name, object value)
        {
            if (mValues == null)
            {
                 mValues = ModelObject.New(this);
            }
            mValues.TrySetValue(name, value);
            return this;
        }

        public Model SetValue(ModelObject obj)
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
                    if (mValues.TryGetValue(p.Name, out value))
                    {
                        DbParameter dp = DbUtils.CreateParam(Path, p.Name,
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
            if (mValues == null)
            {
                throw new Exception("没有任何要保存的内容，请通过SetValue设置内容。");
            }
            SQLExecuteObj execObj = BuildInsertSQL();
            return DbUtils.ExecuteNonQuery(Path, execObj.Command, execObj.Values.ToArray()) == 1;
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
                    if (mValues.TryGetValue(p.Name, out value))
                    {
                        DbParameter dp = DbUtils.CreateParam(Path, p.Name,
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
            if (mValues == null)
            {
                throw new Exception("没有任何要更新的内容，请通过SetValue设置内容。");
            }

            SQLExecuteObj execObj = BuildUpdateSQL();
            ActionSQL actionSQL = mCondition.Build(this);

            if (string.IsNullOrEmpty(actionSQL.Command) && !updateAll)
            {
                throw new Exception("未设置更新的条件范围。");
            }

            if (!string.IsNullOrEmpty(actionSQL.Command))
            {
                execObj.Command += string.Concat(" WHERE ", actionSQL.Command);
                execObj.Values.AddRange(actionSQL.Params);
            }

            return DbUtils.ExecuteNonQuery(Path, execObj.Command, execObj.Values.ToArray()) > 0;
        }
    }
}
