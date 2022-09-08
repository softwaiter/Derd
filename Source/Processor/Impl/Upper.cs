namespace CodeM.Common.Orm.Processors
{
    public class Upper : IProcessor
    {
        public dynamic Execute(Model model, string key, dynamic value)
        {
            if (value != null && value.GetType() == typeof(string))
            {
                return value.ToUpper();
            }
            return value;
        }
    }
}
