namespace CodeM.Common.Orm
{
    public interface ICondition
    {

        ICondition And(ICondition subCondition);

        ICondition Or(ICondition subCondition);

        ICondition Equals(string name, object value);

        ICondition NotEquals(string name, object value);

    }
}