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

        IFilter Equals(string prop, object value);

        IFilter Equals(Function function, object value);

        IFilter NotEquals(string prop, object value);

        IFilter NotEquals(Function function, object value);

        IFilter Gt(string prop, object value);

        IFilter Gt(Function function, object value);

        IFilter Gte(string prop, object value);

        IFilter Gte(Function function, object value);

        IFilter Lt(string prop, object value);

        IFilter Lt(Function function, object value);

        IFilter Lte(string prop, object value);

        IFilter Lte(Function function, object value);

        IFilter Like(string prop, object value);

        IFilter Like(Function function, object value);

        IFilter NotLike(string prop, object value);

        IFilter NotLike(Function function, object value);

        IFilter IsNull(string prop);

        IFilter IsNull(Function function);

        IFilter IsNotNull(string prop);

        IFilter IsNotNull(Function function);

        IFilter Between(string prop, object value, object value2);

        IFilter Between(Function function, object value, object value2);

        IFilter In(string prop, params object[] values);

        IFilter In(Function function, params object[] values);

        IFilter NotIn(string prop, params object[] values);

        IFilter NotIn(Function function, params object[] values);
    }
}