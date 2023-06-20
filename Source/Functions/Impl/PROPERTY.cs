using System;

namespace CodeM.Common.Orm
{
    /// <summary>
    /// 表示value值是一个Model的Property名称
    /// </summary>
    internal class PROPERTY : Function
    {
        private Property mProperty;
        internal PROPERTY(object value, Property p) : base(value)
        {
            if (value is Function)
            {
                throw new ArgumentException("value");
            }

            mProperty = p;
        }

        internal override bool IsProperty()
        {
            return true;
        }

        internal string Value
        {
            get
            {
                return ("" + this.Arguments[0]).Trim();
            }
        }

        internal Property Property
        {
            get
            {
                return mProperty;
            }
        }
    }
}
