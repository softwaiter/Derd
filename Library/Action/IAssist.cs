namespace CodeM.Common.Orm.Action
{
    public interface IAssist
    {
        Model SelectForUpdate();

        Model NoWait();
    }
}
