namespace CodeM.Common.Orm
{
    public interface IModelProcessor
    {
        public bool Process(Model modelDefine, dynamic input, dynamic output, int? transCode = null);
    }
}
