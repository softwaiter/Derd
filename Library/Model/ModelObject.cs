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
                    bool hasValue = true;

                    if (mValues.ContainsKey(p.Name))
                    {
                        DbParameter dp = DbUtils.CreateParam(mModel.Path, p.Name,
                            mValues[p.Name], p.FieldType, ParameterDirection.Input);
                        paramList.Add(dp);
                    }
                    else
                    {
                        if (!Undefined.IsUndefined(p.DefaultValue))
                        {
                            DbParameter dp = DbUtils.CreateParam(mModel.Path, p.Name,
                                p.DefaultValue, p.FieldType, ParameterDirection.Input);
                            paramList.Add(dp);
                        }
                        else
                        {
                            hasValue = false;
                        }
                    }

                    if (hasValue)
                    {
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

        public void Update()
        { 
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
