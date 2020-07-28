namespace CodeM.Common.Orm
{
    public interface IFilter
    {

        IFilter And(IFilter subCondition);

        IFilter Or(IFilter subCondition);

        IFilter Equals(string name, object value);

        IFilter NotEquals(string name, object value);

    }
}