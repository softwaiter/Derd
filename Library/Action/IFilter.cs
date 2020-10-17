namespace CodeM.Common.Orm
{
    public interface IFilter
    {

        IFilter Parent { get; set; }

        IFilter And(IFilter subFilter);

        IFilter Or(IFilter subFilter);

        IFilter Equals(string name, object value);

        IFilter NotEquals(string name, object value);

        IFilter Gt(string name, object value);

        IFilter Gte(string name, object value);

        IFilter Lt(string name, object value);

        IFilter Lte(string name, object value);

        IFilter Like(string name, string value);

        IFilter NotLike(string name, string value);

        IFilter IsNull(string name);

        IFilter IsNotNull(string name);

        IFilter Between(string name, object value, object value2);

    }
}