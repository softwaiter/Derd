namespace CodeM.Common.Orm
{
    public interface IFilter
    {
        bool IsEmpty();

        IFilter Parent { get; set; }

        IFilter And(string condition);

        IFilter And(IFilter subFilter);

        IFilter Or(string condition);

        IFilter Or(IFilter subFilter);

        IFilter Equals(string name, object value);

        IFilter Equals(Function function, object value);

        IFilter NotEquals(string name, object value);

        IFilter NotEquals(Function function, object value);

        IFilter Gt(string name, object value);

        IFilter Gt(Function function, object value);

        IFilter Gte(string name, object value);

        IFilter Gte(Function function, object value);

        IFilter Lt(string name, object value);

        IFilter Lt(Function function, object value);

        IFilter Lte(string name, object value);

        IFilter Lte(Function function, object value);

        IFilter Like(string name, object value);

        IFilter Like(Function function, object value);

        IFilter NotLike(string name, object value);

        IFilter NotLike(Function function, object value);

        IFilter IsNull(string name);

        IFilter IsNull(Function function);

        IFilter IsNotNull(string name);

        IFilter IsNotNull(Function function);

        IFilter Between(string name, object value, object value2);

        IFilter Between(Function function, object value, object value2);

        IFilter In(string name, params object[] values);

        IFilter In(Function function, params object[] values);

        IFilter NotIn(string name, params object[] values);

        IFilter NotIn(Function function, params object[] values);
    }
}