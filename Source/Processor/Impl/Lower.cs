namespace CodeM.Common.Orm.Processors
{
    public class Lower : IProcessor
    {
        public dynamic Execute(Model model, string key, dynamic value)
        {
            if (value != null && value.GetType() == typeof(string))
            {
                return value.ToLower();
            }
            return value;
        }
    }
}
