using CodeM.Common.Orm.SQL.Dialect;
using System;

namespace CodeM.Common.Orm
{
    internal class DATETIME : Function
    {
        public DATETIME(object value) : base(value)
        {
        }

        public DATETIME(Function function) : base(function) { }

        internal override string Convert2SQL(Model m)
        {
            if (this.Arguments.Length == 1 && this.Arguments[0] != null)
            {
                return base.Convert2SQL(m);
            }
            else
            {
                string funcName = this.GetType().Name.ToUpper();
                object[] args = new object[] { $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'" };
                return Features.GetFunctionCommand(m, funcName, args);
            }
        }
    }
}
