using System;
using System.Collections.Generic;
using System.Data;
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

        public bool TrySetValue(string name, object value)
        {
            Property p = mModel.GetProperty(name);
            if (p.IsNotNull && value == null)
            {
                throw new NoNullAllowedException(name);
            }
            if (p.Type == typeof(string) && value != null && p.Length > 0)
            {
                if (value.ToString().Length > p.Length)
                {
                    throw new Exception(string.Concat("属性“", name, "”的最大长度 ", p.Length));
                }
            }
            mValues[name] = value;
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

        public bool TryGetValue(string name, out object result)
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

        public bool Has(string key)
        {
            return mValues.ContainsKey(key);
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
