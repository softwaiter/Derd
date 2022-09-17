namespace CodeM.Common.Orm
{
    public interface IRuleProcessor
    {
        public bool Validate(Model modelDefine, dynamic modelObj);
    }
}
