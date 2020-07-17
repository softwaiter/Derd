using CodeM.Common.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;

namespace CodeM.Common.Orm
{

    public enum SQLCommandType
    {
        Insert = 0,
        Delete = 1,
        Update = 2
    }

    internal class SQLExecuteObj
    {
        public SQLExecuteObj(SQLCommandType type)
        {
            this.CommandType = type;
        }

        public string Command { get; set; }

        public SQLCommandType CommandType { get; set; }

        public List<DbParameter> Values { get; set; }

        public List<DbParameter> Wheres { get; set; }
    }

    public class ModelObject : DynamicObject
    {
        private Model mModel;
        Dictionary<string, object> mValues = new Dictionary<string, object>();

        private ModelObject(Model model)
        {
            mModel = model;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < mModel.PropertyCount; i++)
            {
                names.Add(mModel.GetProperty(i).Name);
            }
            return names;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Property p = mModel.GetProperty(binder.Name);
            if (p.IsNotNull && value == null)
            {
                throw new NoNullAllowedException(binder.Name);
            }
            if (p.Type == typeof(string) && value != null && p.Length > 0)
            {
                if (value.ToString().Length > p.Length)
                {
                    throw new Exception(string.Concat("属性“", binder.Name, "”的最大长度 ", p.Length));
                }
            }
            mValues[binder.Name] = value;
            return true;
        }
        
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (mValues.ContainsKey(binder.Name))
            {
                result = mValues[binder.Name];
            }
            else
            {
                Property p = mModel.GetProperty(binder.Name);
                if (Undefined.IsUndefined(p.DefaultValue))
                {
                    result = null;
                }
                else
                {
                    result = p.DefaultValue;
                }
            }
            return true;
        }

        internal bool TryGetValue(string name, out object result)
        {
            if (mValues.ContainsKey(name))
            {
                result = mValues[name];
            }
            else
            {
                Property p = mModel.GetProperty(name);
                result = p.DefaultValue;
            }
            return result != Undefined.Value;
        }

        internal SQLExecuteObj BuildSQLCommand(SQLCommandType type = SQLCommandType.Insert, string uniqueGroup = null)
        {
            switch (type)
            {
                case SQLCommandType.Delete:
                    return null;
                case SQLCommandType.Update:
                    return BuildUpdateSQL(uniqueGroup);
                default:
                    return BuildInsertSQL();
            }
        }

        private SQLExecuteObj BuildInsertSQL()
        {
            SQLExecuteObj result = new SQLExecuteObj(SQLCommandType.Insert);
            result.Values = new List<DbParameter>();

            string insertFields = string.Empty;
            string insertValues = string.Empty;
            for (int i = 0; i < mModel.PropertyCount; i++)
            {
                Property p = mModel.GetProperty(i);

                if (p.JoinInsert)
                {
                    object value;
                    if (TryGetValue(p.Name, out value))
                    {
                        DbParameter dp = DbUtils.CreateParam(mModel.Path, p.Name,
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
            result.Command = string.Concat("INSERT INTO ", mModel.Table, " (", insertFields, ") VALUES(", insertValues + ")");

            return result;
        }

        private SQLExecuteObj BuildUpdateSQL(string uniqueGroup = null)
        {
            string whereFields;
            if (uniqueGroup != null)
            {
                whereFields = mModel.GetUniqueGroupFields(uniqueGroup);
                if (string.IsNullOrWhiteSpace(whereFields))
                {
                    throw new Exception(string.Concat("未找到Unique Group定义：", uniqueGroup));
                }
            }
            else
            {
                whereFields = mModel.GetPrimaryFields();
            }

            if (!string.IsNullOrWhiteSpace(whereFields))
            {
                SQLExecuteObj execObj = new SQLExecuteObj(SQLCommandType.Update);
                execObj.Values = new List<DbParameter>();
                execObj.Wheres = new List<DbParameter>();

                whereFields = string.Concat(whereFields.ToLower(), ",");

                string updateContent = string.Empty;
                string whereCondition = string.Empty;
                for (int i = 0; i < mModel.PropertyCount; i++)
                {
                    Property p = mModel.GetProperty(i);

                    object value;
                    if (whereFields.Contains(p.Name.ToLower() + ","))
                    {
                        if (TryGetValue(p.Name, out value))
                        {
                            DbParameter dp = DbUtils.CreateParam(mModel.Path, p.Name,
                                value, p.FieldType, ParameterDirection.Input);
                            execObj.Wheres.Add(dp);

                            if (whereCondition.Length > 0)
                            {
                                whereCondition += " AND ";
                            }
                            whereCondition += string.Concat("(", p.Field, "=?)");
                        }
                        else
                        {
                            throw new Exception(string.Concat("缺少定位字段数据，无法定位要更新的数据。"));
                        }
                    }
                    else
                    {
                        if (p.JoinUpdate)
                        {
                            if (TryGetValue(p.Name, out value))
                            {
                                DbParameter dp = DbUtils.CreateParam(mModel.Path, p.Name,
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
                }
                execObj.Command = string.Concat("UPDATE ", mModel.Table, " SET ", updateContent, " WHERE ", whereCondition);

                return execObj;
            }

            throw new Exception("缺少主键定义信息，无法定位要更新的数据。");
        }

        private SQLExecuteObj BuildDeleteSQL(string uniqueGroup = null)
        {
            string whereFields;
            if (uniqueGroup != null)
            {
                whereFields = mModel.GetUniqueGroupFields(uniqueGroup);
                if (string.IsNullOrWhiteSpace(whereFields))
                {
                    throw new Exception(string.Concat("未找到Unique Group定义：", uniqueGroup));
                }
            }
            else
            {
                whereFields = mModel.GetPrimaryFields();
            }

            if (!string.IsNullOrWhiteSpace(whereFields))
            {
                SQLExecuteObj execObj = new SQLExecuteObj(SQLCommandType.Delete);
                execObj.Wheres = new List<DbParameter>();

                object value;
                string whereCondition = string.Empty;
                string[] whereFieldItems = whereFields.Split(',');
                foreach (string field in whereFieldItems)
                {
                    Property p = mModel.GetPropertyByField(field);
                    if (TryGetValue(p.Name, out value))
                    {
                        DbParameter dp = DbUtils.CreateParam(mModel.Path, p.Name,
                            value, p.FieldType, ParameterDirection.Input);
                        execObj.Wheres.Add(dp);

                        if (whereCondition.Length > 0)
                        {
                            whereCondition += " AND ";
                        }
                        whereCondition += string.Concat("(", p.Field, "=?)");
                    }
                    else
                    {
                        throw new Exception(string.Concat("缺少定位字段数据，无法定位要删除的数据。"));
                    }
                }
                execObj.Command = string.Concat("DElETE FROM ", mModel.Table, " WHERE ", whereCondition);

                return execObj;
            }

            throw new Exception("缺少主键定义信息，无法定位要删除的数据。");
        }

        /// <summary>
        /// 将对象信息保存到数据库
        /// </summary>
        /// <param name="replace">对已存在的记录是否替换，默认false</param>
        /// <returns></returns>
        public bool Save()
        {
            SQLExecuteObj execObj = BuildSQLCommand();
            return DbUtils.ExecuteNonQuery(mModel.Path, execObj.Command, execObj.Values.ToArray()) == 1;
        }

        /// <summary>
        /// 将对象信息更新到数据库
        /// </summary>
        /// <param name="uniqueGroup">匹配更新对象的约束条件，默认未设置；使用主键信息</param>
        /// <returns></returns>
        public bool Update(string uniqueGroup=null)
        {
            SQLExecuteObj seo = BuildUpdateSQL(uniqueGroup);
            List<DbParameter> updateParams = new List<DbParameter>();
            updateParams.AddRange(seo.Values);
            updateParams.AddRange(seo.Wheres);
            return DbUtils.ExecuteNonQuery(mModel.Path, seo.Command, updateParams.ToArray()) > 0;
        }

        /// <summary>
        /// 删除和当前对象匹配的数据记录
        /// </summary>
        /// <param name="uniqueGroup">匹配删除对象的约束条件，默认未设置；使用主键信息</param>
        /// <returns></returns>
        public bool Delete(string uniqueGroup = null)
        {
            SQLExecuteObj seo = BuildDeleteSQL(uniqueGroup);
            return DbUtils.ExecuteNonQuery(mModel.Path, seo.Command, seo.Wheres.ToArray()) > 0;
        }

        public static ModelObject New(string modelName)
        {
            Model model = ModelUtils.GetModel(modelName);
            return New(model);
        }

        public static ModelObject New(Model model)
        {
            return new ModelObject(model);
        }
    }
}
