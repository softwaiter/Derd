namespace CodeM.Common.Orm
{
    /// <summary>
    /// 表示value值是一个Model的Property名称
    /// </summary>
    internal class PROPERTY : Function
    {
        internal PROPERTY(string value)
            : base(value)
        {
        }

        internal override bool IsProperty()
        {
            return true;
        }

        internal string Value
        {
            get
            {
                return ("" + this.Arguments[0]).Trim();
            }
        }

        internal Property Resolve(Model m)
        {
            return m.GetProperty(this.Value);
        }
    }
}
