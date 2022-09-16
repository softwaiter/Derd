namespace CodeM.Common.Orm
{
    public class NotSet
    {
        private NotSet()
        { 
        }

        public static NotSet Value = new NotSet();

        public static bool IsNotSetValue(object obj)
        {
            return obj != null && obj.GetType() == typeof(NotSet);
        }
    }
}
