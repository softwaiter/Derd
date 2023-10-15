using System;

namespace CodeM.Common.Orm
{
    internal class DISTINCT : Function
    {
        public DISTINCT(object value) : base(value)
        {
        }

        public DISTINCT(Function function) : base(function)
        {
        }

        internal override bool IsDistinct()
        {
            return true;
        }

        internal override string Convert2SQL(Model m)
        {
            string result = base.Convert2SQL(m);
            ConnectionSetting cs = ConnectionUtils.GetConnectionByModel(m);
            if ("sqlserver".Equals(cs.Dialect, StringComparison.OrdinalIgnoreCase))
            {
                return result.Replace("DISTINCT", "DISTINCT TOP 9223372036854775807");
            }
            return result;
        }
    }
}