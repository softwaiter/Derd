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

        IFilter Equals(object key, object value);

        IFilter Equals(Function function, object value);

        IFilter NotEquals(object key, object value);

        IFilter NotEquals(Function function, object value);

        IFilter Gt(object key, object value);

        IFilter Gt(Function function, object value);

        IFilter Gte(object key, object value);

        IFilter Gte(Function function, object value);

        IFilter Lt(object key, object value);

        IFilter Lt(Function function, object value);

        IFilter Lte(object key, object value);

        IFilter Lte(Function function, object value);

        IFilter Like(object key, object value);

        IFilter Like(Function function, object value);

        IFilter NotLike(object key, object value);

        IFilter NotLike(Function function, object value);

        IFilter IsNull(object key);

        IFilter IsNull(Function function);

        IFilter IsNotNull(object key);

        IFilter IsNotNull(Function function);

        IFilter Between(object key, object value, object value2);

        IFilter Between(Function function, object value, object value2);

        IFilter In(object key, params object[] values);

        IFilter In(Function function, params object[] values);

        IFilter NotIn(object key, params object[] values);

        IFilter NotIn(Function function, params object[] values);
    }
}