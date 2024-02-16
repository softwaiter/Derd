using System;

namespace CodeM.Common.Orm
{
    internal class DATETIME : Function
    {
        public DATETIME(object value) : base(value) { }

        public DATETIME(Function function) : base(function) { }

        internal override string Convert2SQL(Model m)
        {
            if (this.Arguments.Length == 1 && this.Arguments[0] != null)
            {
                if (CommandUtils.IsProperty(m, this.Arguments[0]))
                {
                    return base.Convert2SQL(m);
                }
                else
                {
                    if (DateTime.TryParse(this.Arguments[0].ToString(), out var datetime))
                    {
                        return string.Concat("'", datetime.ToString("yyyy-MM-dd HH:mm:ss"), "'");
                    }
                    else
                    {
                        return string.Concat("'", this.Arguments[0], "'");
                    }
                }
            }
            else
            {
                return string.Concat("'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "'");
            }
        }
    }
}
