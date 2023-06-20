using System;

namespace CodeM.Common.Orm.SQL
{
    internal class GroupByPart
    {
        private Function mFunction;

        public GroupByPart(Function function)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }
            mFunction = function;
        }

        public Function Function { get { return mFunction; } }

        public string Convert2SQL(Model m)
        { 
            return mFunction.Convert2SQL(m);
        }
    }
}
