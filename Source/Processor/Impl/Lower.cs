namespace CodeM.Common.Orm.Processors
{
    public class Lower : IPropertyProcessor
    {
        public object Process(Model modelDefine, string propName, dynamic propValue)
        {
            if (propValue != null && propValue.GetType() == typeof(string))
            {
                return propValue.ToLower();
            }
            return propValue;
        }
    }
}
