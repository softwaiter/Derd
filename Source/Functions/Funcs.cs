namespace CodeM.Common.Orm
{
    public class Funcs
    {
        public static Function VALUE(object value)
        {
            return new VALUE(value);
        }

        public static Function DATE(object value)
        {
            return new DATE(value);
        }

        public static Function DATE(Function function)
        {
            return new DATE(function);
        }

        public static Function COUNT(object value)
        {
            return new COUNT(value);
        }

        public static Function COUNT(Function function)
        {
            return new COUNT(function);
        }

        public static Function DISTINCT(object value)
        {
            return new DISTINCT(value);
        }

        public static Function DISTINCT(Function function)
        {
            return new DISTINCT(function);
        }

        public static Function SUM(object value)
        {
            return new SUM(value);
        }

        public static Function SUM(Function function)
        {
            return new SUM(function);
        }

        public static Function MAX(object value)
        {
            return new MAX(value);
        }

        public static Function MAX(Function function)
        {
            return new MAX(function);
        }

        public static Function MIN(object value)
        {
            return new MIN(value);
        }

        public static Function MIN(Function function)
        {
            return new MIN(function);
        }

        public static Function AVG(object value)
        {
            return new AVG(value);
        }

        public static Function AVG(Function function)
        {
            return new AVG(function);
        }
    }

}
