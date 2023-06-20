namespace CodeM.Common.Orm
{
    internal class UNDECIDED : Function
    {
        public UNDECIDED(object value) : base(value)
        {
        }

        internal override bool IsUndefined()
        {
            return true; 
        }

        internal Function Resolve(Model m)
        {
            object value = this.Arguments[0];
            if (value.GetType() == typeof(string))
            {
                if (CommandUtils.IsProperty(m, value, out Property p))
                {
                    return new PROPERTY(value, p);
                }
                else
                {
                    return new VALUE(value);
                }
            }
            else
            {
                return new VALUE(value);
            }            
        }
    }
}
