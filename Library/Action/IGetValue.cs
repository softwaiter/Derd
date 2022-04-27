using static CodeM.Common.Orm.Model;

namespace CodeM.Common.Orm
{
    public interface IGetValue
    {
        Model GetValue(AggregateType aggType, string name, string alias = null);

        Model GetValue(FunctionType funcType, string name, string alias = null);

        Model GetValue(params string[] names);

    }
}
