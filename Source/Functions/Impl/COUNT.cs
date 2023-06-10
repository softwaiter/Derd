namespace CodeM.Common.Orm
{
    public class COUNT : Function
    {
        public COUNT(string propName) : base(propName)
        {
        }

        public COUNT(Function function) : base(function)
        {
        }
    }
}
