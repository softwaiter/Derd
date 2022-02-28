using CodeM.Common.DbHelper;
using CodeM.Common.Orm.Dialect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

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
        Between = 4096,
        In = 8192,
        NotIn = 16384
    }

    [Serializable]
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

        public IFilter In(string name, params object[] values)
        {
            if (values.Length == 1)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Equals,
                    new KeyValuePair<string, object>(name, values[0])));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.In,
                    new KeyValuePair<string, object>(name, values)));
            }
            return this;
        }

        public IFilter NotIn(string name, params object[] values)
        {
            if (values.Length == 1)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotEquals,
                    new KeyValuePair<string, object>(name, values[0])));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotIn,
                    new KeyValuePair<string, object>(name, values)));
            }
            return this;
        }

        internal CommandSQL Build(Model model)
        {
            string[] quotes = Features.GetObjectQuotes(model);

            CommandSQL result = new CommandSQL();

            KeyValuePair<string, object> expr = new KeyValuePair<string, object>();
            Model currM;
            Property p = null;
            DbParameter dp, dp2;
            foreach (KeyValuePair<FilterOperator, object> item in mFilterItems)
            {
                currM = model;

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

                string paramName = CommandUtils.GenParamName(p);
                string paramPlaceholder = Features.GetCommandParamName(currM, paramName);
                string paramName2 = CommandUtils.GenParamName(p);
                string paramPlaceholder2 = Features.GetCommandParamName(currM, paramName2);

                DbType dbType = CommandUtils.GetDbParamType(p);

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
                        result.ForeignTables.AddRange(andActionSQL.ForeignTables);
                        break;
                    case FilterOperator.Or:
                        if (!string.IsNullOrWhiteSpace(result.SQL))
                        {
                            result.SQL += " OR ";
                        }
                        CommandSQL orActionSQL = ((SubFilter)item.Value).Build(model);
                        result.SQL += orActionSQL.SQL;
                        result.Params.AddRange(orActionSQL.Params);
                        result.ForeignTables.AddRange(orActionSQL.ForeignTables);
                        break;
                    case FilterOperator.Equals:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                expr.Value, dbType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                p.DoBeforeSaveProcessor(inputObj), dbType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], "=", paramPlaceholder);
                        break;
                    case FilterOperator.NotEquals:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                expr.Value, dbType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                p.DoBeforeSaveProcessor(inputObj), dbType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], "<>", paramPlaceholder);
                        break;
                    case FilterOperator.Gt:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                expr.Value, dbType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                p.DoBeforeSaveProcessor(inputObj), dbType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], ">", paramPlaceholder);
                        break;
                    case FilterOperator.Gte:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                expr.Value, dbType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                p.DoBeforeSaveProcessor(inputObj), dbType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], ">=", paramPlaceholder);
                        break;
                    case FilterOperator.Lt:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                expr.Value, dbType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                p.DoBeforeSaveProcessor(inputObj), dbType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], "<", paramPlaceholder);
                        break;
                    case FilterOperator.Lte:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                expr.Value, dbType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                p.DoBeforeSaveProcessor(inputObj), dbType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], "<=", paramPlaceholder);
                        break;
                    case FilterOperator.Like:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                expr.Value, DbType.String, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                p.DoBeforeSaveProcessor(inputObj), DbType.String, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], " LIKE ", paramPlaceholder);
                        break;
                    case FilterOperator.NotLike:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                expr.Value, DbType.String, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                p.DoBeforeSaveProcessor(inputObj), DbType.String, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], " NOT LIKE ", paramPlaceholder);
                        break;
                    case FilterOperator.IsNull:
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], " IS NULL");
                        break;
                    case FilterOperator.IsNotNull:
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], " IS NOT NULL");
                        break;
                    case FilterOperator.Between:
                        object[] values = (object[])expr.Value;
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                values[0], dbType, ParameterDirection.Input);
                            dp2 = DbUtils.CreateParam(currM.Path, paramName2,
                                values[1], dbType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = values[0];
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                p.DoBeforeSaveProcessor(inputObj), dbType, ParameterDirection.Input);

                            dynamic inputObj2 = currM.NewObject();
                            inputObj2[p.Name] = values[1];
                            dp2 = DbUtils.CreateParam(currM.Path, paramName2,
                                p.DoBeforeSaveProcessor(inputObj2), dbType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.Params.Add(dp2);
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], " BETWEEN ", paramPlaceholder, " AND ", paramPlaceholder2);
                        break;
                    case FilterOperator.In:
                        StringBuilder sbInSQL = new StringBuilder(string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], " IN("));
                        object[] items = (object[])expr.Value;
                        for (int i = 0; i < items.Length; i++)
                        {
                            if (i > 0)
                            {
                                sbInSQL.Append(",");
                            }

                            string inParamName = CommandUtils.GenParamName(p);
                            dp = DbUtils.CreateParam(currM.Path, inParamName,
                                items[i], dbType, ParameterDirection.Input);
                            result.Params.Add(dp);
                            string inParamPlaceholder = Features.GetCommandParamName(currM, inParamName);
                            sbInSQL.Append(inParamPlaceholder);
                        }
                        sbInSQL.Append(")");
                        result.SQL += sbInSQL.ToString();
                        break;
                    case FilterOperator.NotIn:
                        StringBuilder sbNotInSQL = new StringBuilder(string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], " NOT IN("));
                        object[] notItems = (object[])expr.Value;
                        for (int i = 0; i < notItems.Length; i++)
                        {
                            if (i > 0)
                            {
                                sbNotInSQL.Append(",");
                            }

                            string notInParamName = CommandUtils.GenParamName(p);
                            dp = DbUtils.CreateParam(currM.Path, notInParamName,
                                notItems[i], dbType, ParameterDirection.Input);
                            result.Params.Add(dp);
                            string notInParamPlaceholder = Features.GetCommandParamName(currM, notInParamName);
                            sbNotInSQL.Append(notInParamPlaceholder);
                        }
                        sbNotInSQL.Append(")");
                        result.SQL += sbNotInSQL.ToString();
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
