namespace CodeM.Common.Orm
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
