namespace CodeM.Common.Orm
{
    public interface IFilter
    {

        IFilter And(IFilter subCondition);

        IFilter Or(IFilter subCondition);

        IFilter Equals(string name, object value);

        IFilter NotEquals(string name, object value);

        IFilter Like(string name, string value);

        IFilter NotLike(string name, string value);

        IFilter IsNull(string name);

        IFilter IsNotNull(string name);

        IFilter Between(string name, object value, object value2);

    }
}