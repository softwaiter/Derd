﻿using System;

namespace CodeM.Common.Orm
{
    internal class DATE : Function
    {
        public DATE(object value) : base(value)
        {
        }

        public DATE(Function function) : base(function) { }

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
                    return string.Concat("'", datetime.ToString("yyyy-MM-dd"), "'");
                }
                else
                {
                    return string.Concat("'", this.Arguments[0], "'");
                }
            }
        }
    }
}