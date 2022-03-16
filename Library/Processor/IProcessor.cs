namespace CodeM.Common.Orm
{
    public interface IProcessor
    {
        dynamic Execute(Model model, string key, dynamic value);
    }
}
