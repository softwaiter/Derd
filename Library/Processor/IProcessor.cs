namespace CodeM.Common.Orm
{
    public interface IProcessor
    {
        object Execute(Model model, string prop, dynamic obj);
    }
}
