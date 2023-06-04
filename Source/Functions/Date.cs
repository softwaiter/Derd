using CodeM.Common.Orm.Functions.Impl;

namespace CodeM.Common.Orm.Functions
{
    public class Date
    {
        public static Function DATE(string name)
        {
            return new DATE(name);
        }

        public static Function DATE(Function function)
        {
            return new DATE(function);
        }
    }
}
