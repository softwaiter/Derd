namespace CodeM.Common.Orm
{
    internal class SUBSTR : Function
    {
        public SUBSTR(object value, int start, int len) 
            : base(value, start, len)
        {
        }

        public SUBSTR(Function function, int start, int len) 
            : base(function, start, len)
        {
        }
    }
}
