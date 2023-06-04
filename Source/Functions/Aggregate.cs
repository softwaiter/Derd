using CodeM.Common.Orm.Functions.Impl;

namespace CodeM.Common.Orm.Functions
{
    public class Aggregate
    {
        public static Function COUNT(string name)
        {
            return new COUNT(name);
        }

        public static Function COUNT(Function function)
        {
            return new COUNT(function);
        }

        public static Function DISTINCT(string name)
        {
            return new DISTINCT(name);
        }

        public static Function DISTINCT(Function function)
        {
            return new DISTINCT(function);
        }

        public static Function SUM(string name)
        {
            return new SUM(name);
        }

        public static Function SUM(Function function)
        {
            return new SUM(function);
        }

        public static Function MAX(string name)
        {
            return new MAX(name);
        }

        public static Function MAX(Function function)
        {
            return new MAX(function);
        }

        public static Function MIN(string name)
        {
            return new MIN(name);
        }

        public static Function MIN(Function function)
        {
            return new MIN(function);
        }

        public static Function AVG(string name)
        {
            return new AVG(name);
        }

        public static Function AVG(Function function)
        {
            return new AVG(function);
        }
    }
}
