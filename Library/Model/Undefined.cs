namespace CodeM.Common.Orm
{
    internal class Undefined
    {
        private static Undefined sSingleton = new Undefined();

        private Undefined()
        {
        }

        public static Undefined Value
        {
            get
            {
                return sSingleton;
            }
        }

        public static bool IsUndefined(object value)
        {
            return sSingleton == value;
        }
    }
}
