namespace CodeM.Common.Orm.Processors
{
    public class Upper : IPropertyProcessor
    {
        public object Process(Model modelDefine, string propName, dynamic propValue)
        {
            if (propValue != null && propValue.GetType() == typeof(string))
            {
                return propValue.ToUpper();
            }
            return propValue;
        }
    }
}
