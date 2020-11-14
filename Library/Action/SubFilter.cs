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
        Gt = 16,
        Gte = 32,
        Lt = 64,
        Lte = 128,
        Like = 256,
        NotLike = 512,
        IsNull = 1024,
        IsNotNull = 2048,
        Between = 4096
    }

    public class SubFilter : IFilter
    {
        private IFilter mParent = null;
        private List<KeyValuePair<FilterOperator, object>> mFilterItems = new List<KeyValuePair<FilterOperator, object>>();

        public void Reset()
        {
            mParent = null;
            mFilterItems.Clear();
        }

        public bool IsEmpty()
        {
            return mFilterItems.Count == 0;
        }

        public IFilter Parent
        {
            get
            {
                return mParent;
            }
            set
            {
                mParent = value;
            }
        }

        public IFilter And(IFilter subFilter)
        {
            subFilter.Parent = this;
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.And, subFilter));
            return this;
        }

        public IFilter Or(IFilter subFilter)
        {
            subFilter.Parent = this;
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Or, subFilter));
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

        public IFilter Gt(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gt,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter Gte(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gte,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter Lt(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lt,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter Lte(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lte,
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

                    if (!expr.Key.Contains("."))
                    {
                        p = model.GetProperty(expr.Key);
                    }
                    else
                    {
                        Model currM = model;

                        string[] subNames = expr.Key.Split(".");
                        for (int i = 0; i < subNames.Length; i++)
                        {
                            string subName = subNames[i];
                            Property subProp = currM.GetProperty(subName);
                            Model subM = ModelUtils.GetModel(subProp.TypeValue);
                            currM = subM;

                            if (i == subNames.Length - 2)
                            {
                                p = subM.GetProperty(subNames[i + 1]);
                                break;
                            }
                        }

                        result.ForeignTables.Add(expr.Key);
                    }

                    if (!string.IsNullOrEmpty(result.SQL))
                    {
                        result.SQL += " AND ";
                    }
                }

                switch (item.Key)
                {
                    case FilterOperator.And:
                        if (!string.IsNullOrWhiteSpace(result.SQL))
                        {
                            result.SQL += " AND ";
                        }
                        CommandSQL andActionSQL = ((SubFilter)item.Value).Build(model);
                        result.SQL += andActionSQL.SQL;
                        result.Params.AddRange(andActionSQL.Params);
                        break;
                    case FilterOperator.Or:
                        if (!string.IsNullOrWhiteSpace(result.SQL))
                        {
                            result.SQL += " OR ";
                        }
                        CommandSQL orActionSQL = ((SubFilter)item.Value).Build(model);
                        result.SQL += orActionSQL.SQL;
                        result.Params.AddRange(orActionSQL.Params);
                        break;
                    case FilterOperator.Equals:
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr.Value, p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.SQL += string.Concat(p.Owner.Table, ".", p.Field, "=?");
                        break;
                    case FilterOperator.NotEquals:
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr.Value, p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.SQL += string.Concat(p.Owner.Table, ".", p.Field, "<>?");
                        break;
                    case FilterOperator.Gt:
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr.Value, p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.SQL += string.Concat(p.Owner.Table, ".", p.Field, ">?");
                        break;
                    case FilterOperator.Gte:
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr.Value, p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.SQL += string.Concat(p.Owner.Table, ".", p.Field, ">=?");
                        break;
                    case FilterOperator.Lt:
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr.Value, p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.SQL += string.Concat(p.Owner.Table, ".", p.Field, "<?");
                        break;
                    case FilterOperator.Lte:
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr.Value, p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.SQL += string.Concat(p.Owner.Table, ".", p.Field, "<=?");
                        break;
                    case FilterOperator.Like:
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr.Value, DbType.String, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.SQL += string.Concat(p.Owner.Table, ".", p.Field, " LIKE ?");
                        break;
                    case FilterOperator.NotLike:
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr.Value, DbType.String, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.SQL += string.Concat(p.Owner.Table, ".", p.Field, " NOT LIKE ?");
                        break;
                    case FilterOperator.IsNull:
                        result.SQL += string.Concat(p.Owner.Table, ".", p.Field, " IS NULL");
                        break;
                    case FilterOperator.IsNotNull:
                        result.SQL += string.Concat(p.Owner.Table, ".", p.Field, " IS NOT NULL");
                        break;
                    case FilterOperator.Between:
                        object[] values = (object[])expr.Value;
                        dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            values[0], p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);
                        dp2 = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            values[1], p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp2);
                        result.SQL += string.Concat(p.Owner.Table, ".", p.Field, " BETWEEN ? AND ?");
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
