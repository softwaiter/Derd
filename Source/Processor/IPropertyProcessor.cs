namespace CodeM.Common.Orm
{
    public interface IPropertyProcessor
    {
        public object Process(Model modelDefine, string propName, dynamic propValue);
    }
}
