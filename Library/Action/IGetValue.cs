using static CodeM.Common.Orm.Model;

namespace CodeM.Common.Orm
{
    public interface IGetValue
    {
        Model GetValue(AggregateType aggType, params string[] names);

        Model GetValue(params string[] names);

    }
}
