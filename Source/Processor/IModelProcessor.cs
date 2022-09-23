namespace CodeM.Common.Orm
{
    public interface IModelProcessor
    {
        public bool Process(Model modelDefine, dynamic modelObj, int? transCode = null);
    }
}
