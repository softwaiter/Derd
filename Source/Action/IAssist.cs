using CodeM.Common.Orm.Functions.Impl;

namespace CodeM.Common.Orm.Action
{
    public interface IAssist
    {
        Model SelectForUpdate();

        Model NoWait();

        Model GroupBy(params string[] names);

        Model GroupBy(Function function);
    }
}
