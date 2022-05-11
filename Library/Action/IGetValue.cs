namespace CodeM.Common.Orm
{
    public interface IGetValue
    {
        Model GetValue(AggregateType aggType, string name, string alias = null);

        Model GetValue(AggregateType aggType, FunctionType funcType, string name, string alias = null);

        Model GetValue(AggregateType aggType, AggregateType aggType2, string name, string alias = null);

        Model GetValue(FunctionType funcType, string name, string alias = null);

        Model GetValue(FunctionType funcType, AggregateType aggType, string name, string alias = null);

        Model GetValue(FunctionType funcType, FunctionType funcType2, string name, string alias = null);

        Model GetValue(params string[] names);

    }
}
