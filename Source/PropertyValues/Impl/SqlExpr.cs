namespace CodeM.Common.Orm
{
    internal class SqlExpr : PropertyValue
    {
        public SqlExpr(string expr) : base(expr) { }

        internal override string Convert2SQL(Property p)
        {
            return ("" + Arguments[0]);
        }
    }
}
