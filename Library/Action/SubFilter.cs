using CodeM.Common.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace CodeM.Common.Orm
{
    public enum FilterOperator
    {
        And = 1,
        Or = 2,
        Equals = 4,
        NotEquals = 8,
        Like = 16,
        NotLike = 32,
        IsNull = 64,
        IsNotNull = 128,
        Between = 256
    }

    public class SubFilter : IFilter
    {
        private List<KeyValuePair<FilterOperator, object>> mFilterItems = new List<KeyValuePair<FilterOperator, object>>();

        public void Reset()
        {
            mFilterItems.Clear();
        }

        public bool IsEmpty()
        {
            return mFilterItems.Count == 0;
        }

        public IFilter And(IFilter subCondition)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.And, subCondition));
            return this;
        }

        public IFilter Or(IFilter subCondition)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Or, subCondition));
            return this;
        }

        public IFilter Equals(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Equals,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter NotEquals(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotEquals,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter Like(string name, string value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Like,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter NotLike(string name, string value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotLike,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter IsNull(string name)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.IsNull,
                new KeyValuePair<string, object>(name, true)));
            return this;
        }

        public IFilter IsNotNull(string name)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.IsNotNull,
                new KeyValuePair<string, object>(name, true)));
            return this;
        }

        public IFilter Between(string name, object value, object value2)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Between,
                new KeyValuePair<string, object>(name, new object[] { value, value2 })));
            return this;
        }

        internal CommandSQL Build(Model model)
        {
            CommandSQL result = new CommandSQL();

            KeyValuePair<string, object> expr = new KeyValuePair<string, object>();
            Property p = null;
            DbParameter dp, dp2;
            foreach (KeyValuePair<FilterOperator, object> item in mFilterItems)
            {
                if (item.Key != FilterOperator.And &&
                    item.Key != FilterOperator.Or)
                {
                    expr = (KeyValuePair<string, object>)item.Value;
                    p = model.GetProperty(expr.Key);
                    if (p == null)
                    {
                        throw new Exception(string.Concat("无效的属性：", expr.Key));
                    }

                    if (!string.IsNullOrEmpty(result.SQL))
                    {
                        result.SQL += " AND ";
                    }
                }

                switch (item.Key)
                {
                    case FilterOperator.And:
                        CommandSQL andActionSQL = ((SubFilter)item.Value).Build(model);
                        result.SQL += string.Concat(" AND ", andActionSQL.SQL);
                        result.Params.AddRange(andActionSQL.Params);
                        break;
                    case FilterOperator.Or:
                        CommandSQL orActionSQL = ((SubFilter)item.Value).Build(model);
                        result.SQL += string.Concat(" OR ", orActionSQL.SQL);
                        result.Params.AddRange(orActionSQL.Params);
                        break;
                    case FilterOperator.Equals:
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr.Value, p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.SQL += string.Concat(p.Field, "=?");
                        break;
                    case FilterOperator.NotEquals:
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr.Value, p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.SQL += string.Concat(p.Field, "<>?");
                        break;
                    case FilterOperator.Like:
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr.Value, DbType.String, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.SQL += string.Concat(p.Field, " LIKE ?");
                        break;
                    case FilterOperator.NotLike:
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr.Value, DbType.String, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.SQL += string.Concat(p.Field, " NOT LIKE ?");
                        break;
                    case FilterOperator.IsNull:
                        result.SQL += string.Concat(p.Field, " IS NULL");
                        break;
                    case FilterOperator.IsNotNull:
                        result.SQL += string.Concat(p.Field, " IS NOT NULL");
                        break;
                    case FilterOperator.Between:
                        object[] values = (object[])expr.Value;
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            values[0], p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);
                        dp2 = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            values[1], p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp2);
                        result.SQL += string.Concat(p.Field, " BETWEEN ? AND ?");
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(result.SQL))
            {
                result.SQL = string.Concat("(", result.SQL, ")");
            }

            return result;
        }

    }
}
