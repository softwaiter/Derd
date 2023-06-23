using System;

namespace CodeM.Common.Orm
{
    internal class TIME : Function
    {
        public TIME(object value) : base(value) { }

        public TIME(Function function) : base(function) { }

        internal override string Convert2SQL(Model m)
        {
            if (CommandUtils.IsProperty(m, this.Arguments[0]))
            {
                return base.Convert2SQL(m);
            }
            else
            {
                if (DateTime.TryParse(this.Arguments[0].ToString(), out var datetime))
                {
                    return string.Concat("'", datetime.ToString("HH:mm:ss"), "'");
                }
                else
                {
                    return string.Concat("'", this.Arguments[0], "'");
                }
            }
        }
    }
}
