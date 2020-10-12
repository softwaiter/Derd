using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace CodeM.Common.Orm.Serialize
{
    public class ModelObject : ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable, IDictionary<string, object>, IDynamicMetaObjectProvider
    {
        private Model mModel;

        private ModelObject(Model m)
        {
            mModel = m;
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

        private void _CheckProperyValue(string name, object value)
        {
            Property p = mModel.GetProperty(name);

            if (p.IsNotNull && value == null)
            {
                throw new Exception(string.Concat("属性值不能为空：", name));
            }

            if (p.Type == typeof(string) && value != null && p.Length > 0)
            {
                if (value.ToString().Length > p.Length)
                {
                    throw new Exception(string.Concat("属性值最大长度不能超过", p.Length, "：", name));
                }
            }
        }

        #region IDictionary<string, object>

        private Dictionary<string, object> mValues = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            {
                return GetValue(key);
            }
            set
            {
                SetValue(key, value);
            }
        }

        public ICollection<string> Keys => mValues.Keys;

        public ICollection<object> Values => mValues.Values;

        public int Count => mValues.Count;

        public bool IsReadOnly => false;

        public void Add(string key, object value)
        {
            SetValue(key, value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            mValues.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            if (mValues.ContainsKey(item.Key))
            {
                return mValues[item.Key] == item.Value;
            }
            return false;
        }

        public bool ContainsKey(string key)
        {
            return mValues.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            mValues.ToArray().CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return mValues.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return mValues.Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            if (Contains(item))
            {
                return Remove(item.Key);
            }
            return false;
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
        {
            if (ContainsKey(key))
            {
                value = mValues[key];
                return true;
            }
            else
            {
                try
                {
                    Property p = mModel.GetProperty(key);
                    if (!Undefined.IsUndefined(p.DefaultValue))
                    {
                        value = p.DefaultValue;
                        return true;
                    }
                }
                catch
                {
                    ;
                }
            }

            value = null;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mValues.GetEnumerator();
        }

        #endregion

        public bool Has(string property)
        {
            return ContainsKey(property);
        }

        #region IDynamicMetaObjectProvider

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new ModelMetaObject(parameter, BindingRestrictions.Empty, this);
        }

        public object SetValue(string key, object value)
        {
            _CheckProperyValue(key, value);

            if (ContainsKey(key))
            {
                mValues[key] = value;
            }
            else
            {
                mValues.Add(key, value);
            }
            return value;
        }

        public object GetValue(string key)
        {
            if (ContainsKey(key))
            {
                return mValues[key];
            }
            else
            {
                Property p = mModel.GetProperty(key);
                if (p != null)
                {
                    if (!Undefined.IsUndefined(p.DefaultValue))
                    {
                        return p.DefaultValue;
                    }
                }
            }

            throw new Exception(string.Concat("未指定属性值：", key));
        }

        #endregion

    }
}
