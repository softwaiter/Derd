using CodeM.Common.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;

namespace CodeM.Common.Orm
{

    public class ModelObject : DynamicObject
    {
        private Model mModel;
        Dictionary<string, object> mValues = new Dictionary<string, object>();

        private ModelObject(Model model)
        {
            mModel = model;
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

        public bool Save()
        {
            string insertFields = string.Empty;
            string insertValues = string.Empty;
            List<DbParameter> paramList = new List<DbParameter>();
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
                        paramList.Add(dp);

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

            string sql = string.Concat("INSERT INTO ", mModel.Table, " (", insertFields, ") VALUES(", insertValues + ")");
            return DbUtils.ExecuteNonQuery(mModel.Path, sql, paramList.ToArray()) == 1;
        }

        public bool Update(string uniqueGroup=null)
        {
            string whereFields = null;
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
                whereFields = string.Concat(whereFields.ToLower(), ",");

                string updateContent = string.Empty;
                string whereCondition = string.Empty;
                List<DbParameter> updateParams = new List<DbParameter>();
                List<DbParameter> whereParams = new List<DbParameter>();
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
                            whereParams.Add(dp);

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
                                updateParams.Add(dp);

                                if (updateContent.Length > 0)
                                {
                                    updateContent += ",";
                                }
                                updateContent += string.Concat(p.Field, "=?");
                            }
                        }
                    }
                }

                updateParams.AddRange(whereParams);
                string sql = string.Concat("UPDATE ", mModel.Table, " SET ", updateContent, " WHERE ", whereCondition);
                return DbUtils.ExecuteNonQuery(mModel.Path, sql, updateParams.ToArray()) == 1;
            }

            throw new Exception("缺少主键定义信息，无法定位要更新的数据。");
        }

        public void Delete()
        { 
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
