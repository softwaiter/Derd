namespace CodeM.Common.Orm
{
    public class Funcs
    {
        public static Function PROPERTY(string name)
        {
            return new PROPERTY(name);
        }

        public static Function VALUE(object value)
        {
            return new VALUE(value);
        }

        public static Function DATETIME(object value)
        {
            return new DATETIME(value);
        }

        public static Function DATETIME(Function function)
        {
            return new DATETIME(function);
        }

        public static Function DATE(object value)
        {
            return new DATE(value);
        }

        public static Function DATE(Function function)
        {
            return new DATE(function);
        }

        public static Function TIME(object value)
        {
            return new TIME(value);
        }

        public static Function TIME(Function function)
        {
            return new TIME(function);
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

        public static Function SUBSTR(string value, int start, int len)
        {
            return new SUBSTR(value, start, len);
        }

        public static Function SUBSTR(Function function, int start, int len)
        {
            return new SUBSTR(function, start, len);
        }

        public static Function LENGTH(string value)
        {
            return new LENGTH(value);
        }

        public static Function LENGTH(Function function)
        {

            return new LENGTH(function);
        }

        public static Function UPPER(string value)
        {
            return new UPPER(value);
        }

        public static Function UPPER(Function function)
        { 
            return new UPPER(function);
        }

        public static Function LOWER(string value)
        {
            return new LOWER(value);
        }

        public static Function LOWER(Function function)
        { 
            return new LOWER(function);
        }

        public static Function LTRIM(string value)
        {
            return new LTRIM(value);
        }

        public static Function LTRIM(Function function)
        {
            return new LTRIM(function);
        }

        public static Function RTRIM(string value)
        {
            return new RTRIM(value);
        }

        public static Function RTRIM(Function function)
        { 
            return new RTRIM(function);
        }

        public static Function TRIM(string value)
        {
            return new TRIM(value);
        }

        public static Function TRIM(Function function)
        {
            return new TRIM(function);
        }

        public static Function ABS(object value)
        {
            return new ABS(value);
        }

        public static Function ABS(Function function)
        {
            return new ABS(function);
        }

        public static Function ROUND(object value, int percision = 0)
        {
            return new ROUND(value, percision);
        }

        public static Function ROUND(Function function, int percision = 0)
        {
            return new ROUND(function, percision);
        }
    }

}
