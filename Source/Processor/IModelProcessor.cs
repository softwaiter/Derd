namespace CodeM.Common.Orm
{
    public interface IModelProcessor
    {
        public bool Process(Model modelDefine, dynamic modelValues, int? transCode = null);
    }
}
