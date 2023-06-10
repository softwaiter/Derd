namespace CodeM.Common.Orm
{ 
    public class DISTINCT : Function
    {
        public DISTINCT(string name) : base(name)
        {
        }

        public DISTINCT(Function calculator) : base(calculator)
        {
        }
    }
}
