namespace CodeM.Common.Orm.Processor
{
    interface IExecute
    {
        object Execute(Model model, dynamic obj);
    }
}
