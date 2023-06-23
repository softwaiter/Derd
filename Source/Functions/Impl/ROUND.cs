namespace CodeM.Common.Orm
{
    internal class ROUND : Function
    {
        public ROUND(object value, int precision = 0) : base(value, precision) { }

        public ROUND(Function function, int precision = 0) : base(function, precision) { }
    }
}
