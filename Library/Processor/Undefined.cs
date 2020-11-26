namespace CodeM.Common.Orm
{
    public class Undefined
    {
        private Undefined()
        { 
        }

        public static Undefined Value = new Undefined();

        public static bool IsUndefinedValue(object obj)
        {
            return obj != null && obj.GetType() == typeof(Undefined);
        }
    }
}
