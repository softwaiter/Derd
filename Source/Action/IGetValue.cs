using CodeM.Common.Orm.Functions.Impl;

namespace CodeM.Common.Orm
{
    public interface IGetValue
    {
        Model GetValue(string propName, string alias = null);

        Model GetValue(Function function, string alias = null);
    }
}
