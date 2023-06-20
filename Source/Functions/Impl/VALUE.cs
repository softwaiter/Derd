using System;

namespace CodeM.Common.Orm
{
    /// <summary>
    /// 表示value值是单纯的内容表示，而不是一个Model的Property
    /// </summary>
    internal class VALUE : Function
    {
        public VALUE(object value) : base(value)
        {
            if (value is Function)
            {
                throw new ArgumentException("value");
            }
        }

        internal override bool IsValue()
        {
            return true;
        }

        internal object Value
        {
            get
            {
                return Arguments[0];
            }
        }

        internal override string Convert2SQL(Model m)
        {
            if (Value != null && Value is string)
            { 
                return "'" + Value + "'";
            }
            return "" + Value;
        }
    }
}
