namespace CodeM.Common.Orm
{
    public interface IProcessor
    {
        dynamic Execute(Model model, string prop, dynamic propValue);
    }
}
