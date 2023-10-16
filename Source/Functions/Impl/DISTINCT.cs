namespace CodeM.Common.Orm
{
    internal class DISTINCT : Function
    {
        public DISTINCT(object value) : base(value)
        {
        }

        public DISTINCT(Function function) : base(function)
        {
        }

        internal override bool IsDistinct()
        {
            return true;
        }
    }
}