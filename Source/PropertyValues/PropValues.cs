namespace CodeM.Common.Orm
{
    public class PropValues
    {
        public static PropertyValue SqlExpr(string expr)
        {
            return new SqlExpr(expr);
        }

        public static PropertyValue SelfAdd(int step = 1)
        { 
            return new SelfAdd(step);
        }

        public static PropertyValue SelfSubtract(int step = 1)
        {
            return new SelfSubtract(step);
        }

        public static PropertyValue SelfMultiply(int step = 1)
        {
            return new SelfMultiply(step);
        }

        public static PropertyValue SelfDivide(int step = 1)
        {
            return new SelfDivide(step);
        }
    }
}
