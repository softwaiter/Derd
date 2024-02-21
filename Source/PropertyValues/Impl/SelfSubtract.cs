using CodeM.Common.Orm.SQL.Dialect;

namespace CodeM.Common.Orm
{
    /// <summary>
    /// 字段自减操作
    /// </summary>
    internal class SelfSubtract : PropertyValue
    {
        public SelfSubtract(int step) : base(step) { }

        internal override string Convert2SQL(Property p)
        {
            string[] quotes = Features.GetObjectQuotes(p.Owner);
            return string.Concat(quotes[0], p.Field, quotes[1], "-", Arguments[0]);
        }
    }
}
