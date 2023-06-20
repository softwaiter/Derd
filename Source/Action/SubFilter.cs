using CodeM.Common.DbHelper;
using CodeM.Common.Orm.SQL.Dialect;
using CodeM.Common.Tools.DynamicObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace CodeM.Common.Orm
{
    internal enum FilterOperator
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
        private static readonly Dictionary<FilterOperator, string> _operatorSymbols = new Dictionary<FilterOperator, string>()
        {
            { FilterOperator.And, " AND " },
            { FilterOperator.Or, " OR " },
            { FilterOperator.Equals, " = " },
            { FilterOperator.NotEquals, " <> " },
            { FilterOperator.Gt, " > " },
            { FilterOperator.Gte, " >= " },
            { FilterOperator.Lt, " < " },
            { FilterOperator.Lte, " <= " },
            { FilterOperator.Like, " LIKE "},
            { FilterOperator.NotLike, " NOT LIKE "},
            { FilterOperator.IsNull, " IS NULL"},
            { FilterOperator.IsNotNull, " IS NOT NULL" },
            { FilterOperator.Between, " BETWEEN " },
            { FilterOperator.In, " IN " },
            { FilterOperator.NotIn, " NOT IN " }
        };

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

        public IFilter And(string condition)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.And, condition));
            return this;
        }

        public IFilter And(IFilter subFilter)
        {
            subFilter.Parent = this;
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.And, subFilter));
            return this;
        }

        public IFilter Or(string condition)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Or, condition));
            return this;
        }

        public IFilter Or(IFilter subFilter)
        {
            subFilter.Parent = this;
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Or, subFilter));
            return this;
        }

        public new IFilter Equals(object key, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Equals,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Equals,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter Equals(Function function, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Equals,
                    new KeyValuePair<Function, object>(function, value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Equals,
                    new KeyValuePair<Function, object>(function, new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter NotEquals(object key, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotEquals,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotEquals,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter NotEquals(Function function, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotEquals,
                    new KeyValuePair<Function, object>(function, value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotEquals,
                    new KeyValuePair<Function, object>(function, new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter Gt(object key, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gt,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gt,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter Gt(Function function, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gt,
                    new KeyValuePair<Function, object>(function, value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gt,
                    new KeyValuePair<Function, object>(function, new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter Gte(object key, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gte,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gte,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter Gte(Function function, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gte,
                    new KeyValuePair<Function, object>(function, value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gte,
                    new KeyValuePair<Function, object>(function, new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter Lt(object key, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lt,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lt,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter Lt(Function function, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lt,
                    new KeyValuePair<Function, object>(function, value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lt,
                    new KeyValuePair<Function, object>(function, new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter Lte(object key, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lte,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lte,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter Lte(Function function, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lte,
                    new KeyValuePair<Function, object>(function, value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lte,
                    new KeyValuePair<Function, object>(function, new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter Like(object key, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Like,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Like,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter Like(Function function, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Like,
                    new KeyValuePair<Function, object>(function, value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Like,
                    new KeyValuePair<Function, object>(function, new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter NotLike(object key, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotLike,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotLike,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter NotLike(Function function, object value)
        {
            if (value is Function)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotLike,
                    new KeyValuePair<Function, object>(function, value)));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotLike,
                    new KeyValuePair<Function, object>(function, new UNDECIDED(value))));
            }
            return this;
        }

        public IFilter IsNull(object key)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.IsNull,
                new KeyValuePair<Function, object>(new UNDECIDED(key), true)));
            return this;
        }

        public IFilter IsNull(Function function)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.IsNull,
                new KeyValuePair<Function, object>(function, true)));
            return this;
        }

        public IFilter IsNotNull(object key)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.IsNotNull,
                new KeyValuePair<Function, object>(new UNDECIDED(key), true)));
            return this;
        }

        public IFilter IsNotNull(Function function)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.IsNotNull,
                new KeyValuePair<Function, object>(function, true)));
            return this;
        }

        public IFilter Between(object key, object value, object value2)
        {
            object[] values = new object[2];
            if (value is Function)
            {
                values[0] = value;
            }
            else
            {
                values[0] = new UNDECIDED(value);
            }
            if (value2 is Function)
            {
                values[1] = value2;
            }
            else
            { 
                values[1] = new UNDECIDED(value2);
            }

            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Between,
                new KeyValuePair<Function, object>(new UNDECIDED(key), values)));
            return this;
        }

        public IFilter Between(Function function, object value, object value2)
        {
            object[] values = new object[2];
            if (value is Function)
            {
                values[0] = value;
            }
            else
            {
                values[0] = new UNDECIDED(value);
            }
            if (value2 is Function)
            {
                values[1] = value2;
            }
            else
            {
                values[1] = new UNDECIDED(value2);
            }

            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Between,
                new KeyValuePair<Function, object>(function, values)));
            return this;
        }

        private object[] PreprocessArguments(object[] values)
        {
            List<object> result = new List<object>();
            foreach (object value in values)
            {
                if (value is Function)
                {
                    result.Add(value);
                }
                else
                {
                    result.Add(new UNDECIDED(value));
                }
            }
            return result.ToArray();
        }

        public IFilter In(object key, params object[] values)
        {
            if (values.Length == 0)
            {
                throw new ArgumentException("values");
            }

            if (values.Length == 1)
            {
                this.Equals(key, values[0]);
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.In,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), PreprocessArguments(values))));
            }
            return this;
        }

        public IFilter In(Function function, params object[] values)
        {
            if (values.Length == 0)
            {
                throw new ArgumentException("values");
            }

            if (values.Length == 1)
            {
                this.Equals(function, values[0]);
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.In,
                    new KeyValuePair<Function, object>(function, PreprocessArguments(values))));
            }
            return this;
        }

        public IFilter NotIn(object key, params object[] values)
        {
            if (values.Length == 0)
            {
                throw new ArgumentException("values");
            }

            if (values.Length == 1)
            {
                this.NotEquals(key, values[0]);
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotIn,
                    new KeyValuePair<Function, object>(new UNDECIDED(key), PreprocessArguments(values))));
            }
            return this;
        }

        public IFilter NotIn(Function function, params object[] values)
        {
            if (values.Length == 0)
            {
                throw new ArgumentException("values");
            }

            if (values.Length == 1)
            {
                this.NotEquals(function, values[0]);
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotIn,
                    new KeyValuePair<Function, object>(function, PreprocessArguments(values))));
            }
            return this;
        }

        private void BuildSimpleOperatorSQL(CommandSQL result, Model m, FilterOperator op,
            Function exprKey, Function exprValue, bool recordEqualsProperties)
        {
            if (exprKey is PROPERTY && exprValue is VALUE)
            {
                PROPERTY propFunc = (PROPERTY)exprKey;
                if (recordEqualsProperties && op == FilterOperator.Equals)
                {
                    if (!propFunc.Value.Contains("."))
                    {
                        if (!result.FilterProperties.ContainsKey(propFunc.Value))
                        {
                            result.FilterProperties.Add(propFunc.Value, ((VALUE)exprValue).Value);
                        }
                    }
                }

                DbParameter dp;
                DbType dbType = CommandUtils.GetDbParamType(propFunc.Property);
                string paramName = CommandUtils.GenParamName(propFunc.Property);
                string paramPlaceholder = Features.GetCommandParamName(m, paramName);
                if (!propFunc.Property.NeedCalcPreSaveProcessor)
                {
                    dp = DbUtils.CreateParam(m.Path, paramName,
                        ((VALUE)exprValue).Value, dbType, ParameterDirection.Input);
                }
                else
                {
                    dynamic inputObj = new DynamicObjectExt();
                    inputObj.SetValue(propFunc.Property.Name, ((VALUE)exprValue).Value);
                    dp = DbUtils.CreateParam(m.Path, paramName,
                        propFunc.Property.DoPreSaveProcessor(inputObj), 
                        dbType, ParameterDirection.Input);
                }
                result.Params.Add(dp);

                result.SQL += string.Concat(propFunc.Convert2SQL(m), _operatorSymbols[op], paramPlaceholder);
            }
            else if (exprKey is VALUE && exprValue is PROPERTY)
            {
                PROPERTY propFunc = (PROPERTY)exprValue;
                if (recordEqualsProperties && op == FilterOperator.Equals)
                {
                    if (!propFunc.Value.Contains("."))
                    {
                        if (!result.FilterProperties.ContainsKey(propFunc.Value))
                        {
                            result.FilterProperties.Add(propFunc.Value, ((VALUE)exprKey).Value);
                        }
                    }
                }

                DbParameter dp;
                DbType dbType = CommandUtils.GetDbParamType(propFunc.Property);
                string paramName = CommandUtils.GenParamName(propFunc.Property);
                string paramPlaceholder = Features.GetCommandParamName(m, paramName);
                if (!propFunc.Property.NeedCalcPreSaveProcessor)
                {
                    dp = DbUtils.CreateParam(m.Path, paramName,
                        ((VALUE)exprKey).Value, dbType, ParameterDirection.Input);
                }
                else
                {
                    dynamic inputObj = new DynamicObjectExt();
                    inputObj.SetValue(propFunc.Property.Name, ((VALUE)exprKey).Value);
                    dp = DbUtils.CreateParam(m.Path, paramName,
                        propFunc.Property.DoPreSaveProcessor(inputObj),
                        dbType, ParameterDirection.Input);
                }
                result.Params.Add(dp);

                result.SQL += string.Concat(paramPlaceholder, _operatorSymbols[op], propFunc.Convert2SQL(m));
            }
            else
            {
                string exprLeft, exprRight;

                if (exprKey is VALUE)
                {
                    DbType dbType = CommandUtils.Type2DbType(((VALUE)exprKey).Value.GetType());
                    string paramName = CommandUtils.GenParamName();
                    exprLeft = Features.GetCommandParamName(m, paramName);
                    DbParameter dp = DbUtils.CreateParam(m.Path, paramName,
                        ((VALUE)exprKey).Value, dbType, ParameterDirection.Input);
                    result.Params.Add(dp);
                }
                else
                {
                    exprLeft = exprKey.Convert2SQL(m);
                }

                if (exprValue is VALUE)
                {
                    DbType dbType2 = CommandUtils.Type2DbType(((VALUE)exprValue).Value.GetType());
                    string paramName2 = CommandUtils.GenParamName();
                    exprRight = Features.GetCommandParamName(m, paramName2);
                    DbParameter dp2 = DbUtils.CreateParam(m.Path, paramName2,
                        ((VALUE)exprValue).Value, dbType2, ParameterDirection.Input);
                    result.Params.Add(dp2);
                }
                else
                {
                    exprRight = exprValue.Convert2SQL(m);
                }

                result.SQL += string.Concat(exprLeft, _operatorSymbols[op], exprRight);
            }
        }

        private void BuildBetweenOperatorSQL(CommandSQL result, Model m,
            Function exprKey, Function exprValue, Function exprValue2)
        {
            string exprLeft, exprRight, exprRight2;
            PROPERTY propKey = null;

            if (exprKey is VALUE)
            {
                DbType dbType = CommandUtils.Type2DbType(((VALUE)exprKey).Value.GetType());
                string paramName = CommandUtils.GenParamName();
                exprLeft = Features.GetCommandParamName(m, paramName);
                DbParameter dp = DbUtils.CreateParam(m.Path, paramName,
                    ((VALUE)exprKey).Value, dbType, ParameterDirection.Input);
                result.Params.Add(dp);
            }
            else
            {
                exprLeft = exprKey.Convert2SQL(m);

                if (exprKey is PROPERTY)
                {
                    propKey = (PROPERTY)exprKey;
                }
            }

            if (exprValue is VALUE)
            {
                DbParameter dp;
                DbType dbType = CommandUtils.Type2DbType(((VALUE)exprValue).Value.GetType());
                string paramName = CommandUtils.GenParamName();
                exprRight = Features.GetCommandParamName(m, paramName);
                if (propKey == null || !propKey.Property.NeedCalcPreSaveProcessor)
                {
                    dp = DbUtils.CreateParam(m.Path, paramName,
                        ((VALUE)exprValue).Value, dbType, ParameterDirection.Input);
                }
                else
                {
                    dynamic inputObj = new DynamicObjectExt();
                    inputObj.SetValue(propKey.Property.Name, ((VALUE)exprValue).Value);
                    dp = DbUtils.CreateParam(m.Path, paramName,
                        propKey.Property.DoPreSaveProcessor(inputObj),
                        dbType, ParameterDirection.Input);
                }
                result.Params.Add(dp);
            }
            else
            {
                exprRight = exprValue.Convert2SQL(m);
            }

            if (exprValue2 is VALUE)
            {
                DbParameter dp;
                DbType dbType = CommandUtils.Type2DbType(((VALUE)exprValue2).Value.GetType());
                string paramName = CommandUtils.GenParamName();
                exprRight2 = Features.GetCommandParamName(m, paramName);
                if (propKey == null || !propKey.Property.NeedCalcPreSaveProcessor)
                {
                    dp = DbUtils.CreateParam(m.Path, paramName,
                        ((VALUE)exprValue2).Value, dbType, ParameterDirection.Input);
                }
                else
                {
                    dynamic inputObj = new DynamicObjectExt();
                    inputObj.SetValue(propKey.Property.Name, ((VALUE)exprValue2).Value);
                    dp = DbUtils.CreateParam(m.Path, paramName,
                        propKey.Property.DoPreSaveProcessor(inputObj),
                        dbType, ParameterDirection.Input);
                }
                result.Params.Add(dp);
            }
            else
            {
                exprRight2 = exprValue2.Convert2SQL(m);
            }

            result.SQL += string.Concat(exprLeft, " BETWEEN ", exprRight, " AND ", exprRight2);
        }

        private void BuildInOperatorSQL(CommandSQL result, Model m,
            FilterOperator op, Function exprKey, object[] values)
        {
            string exprLeft;
            PROPERTY propKey = null;

            if (exprKey is VALUE)
            {
                DbType dbType = CommandUtils.Type2DbType(((VALUE)exprKey).Value.GetType());
                string paramName = CommandUtils.GenParamName();
                exprLeft = Features.GetCommandParamName(m, paramName);
                DbParameter dp = DbUtils.CreateParam(m.Path, paramName,
                    ((VALUE)exprKey).Value, dbType, ParameterDirection.Input);
                result.Params.Add(dp);
            }
            else
            {
                exprLeft = exprKey.Convert2SQL(m);

                if (exprKey is PROPERTY)
                {
                    propKey = (PROPERTY)exprKey;
                }
            }

            StringBuilder sbInSQL = new StringBuilder(string.Concat(exprLeft, _operatorSymbols[op], "("));

            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                {
                    sbInSQL.Append(",");
                }

                if (values[i] is VALUE)
                {
                    DbParameter dp;
                    DbType dbType = CommandUtils.Type2DbType(((VALUE)values[i]).Value.GetType());
                    string paramName = CommandUtils.GenParamName();
                    string inParamPlaceholder = Features.GetCommandParamName(m, paramName);
                    if (propKey == null || !propKey.Property.NeedCalcPreSaveProcessor)
                    {
                        dp = DbUtils.CreateParam(m.Path, paramName,
                            ((VALUE)values[i]).Value, dbType, ParameterDirection.Input);
                    }
                    else
                    {
                        dynamic inputObj = new DynamicObjectExt();
                        inputObj.SetValue(propKey.Property.Name, ((VALUE)values[i]).Value);
                        dp = DbUtils.CreateParam(m.Path, paramName,
                            propKey.Property.DoPreSaveProcessor(inputObj),
                            dbType, ParameterDirection.Input);
                    }
                    result.Params.Add(dp);
                    sbInSQL.Append(inParamPlaceholder);
                }
                else
                {
                    sbInSQL.Append(((Function)values[i]).Convert2SQL(m));
                }
            }

            sbInSQL.Append(")");
            result.SQL += sbInSQL.ToString();
        }

        internal CommandSQL Build(Model model)
        {
            string[] quotes = Features.GetObjectQuotes(model);

            bool recordEqualsProperties = true;
            CommandSQL result = new CommandSQL();

            KeyValuePair<Function, object> expr, defaultExpr = new KeyValuePair<Function, object>();
            Function exprKey = null, exprValue, exprValue2;
            foreach (KeyValuePair<FilterOperator, object> item in mFilterItems)
            {
                expr = defaultExpr;

                if (item.Key != FilterOperator.And &&
                    item.Key != FilterOperator.Or)
                {
                    expr = (KeyValuePair<Function, object>)item.Value;

                    exprKey = expr.Key;
                    if (exprKey is UNDECIDED)
                    {
                        exprKey = ((UNDECIDED)exprKey).Resolve(model);
                    }
                    CommandUtils.CheckFunctionForeignProperty(model, exprKey, result.ForeignTables);

                    if (!string.IsNullOrEmpty(result.SQL))
                    {
                        result.SQL += " AND ";
                    }
                }
                else if (item.Key == FilterOperator.Or)
                {
                    recordEqualsProperties = false;
                    result.FilterProperties.Clear();
                }

                switch (item.Key)
                {
                    case FilterOperator.And:
                        if (item.Value != null && !string.IsNullOrWhiteSpace(item.Value.ToString()))
                        {
                            if (item.Value is SubFilter)
                            {
                                if (!string.IsNullOrWhiteSpace(result.SQL))
                                {
                                    result.SQL += " AND ";
                                }
                                CommandSQL andActionSQL = ((SubFilter)item.Value).Build(model);
                                result.SQL += andActionSQL.SQL;
                                result.Params.AddRange(andActionSQL.Params);
                                result.ForeignTables.AddRange(andActionSQL.ForeignTables);

                                if (recordEqualsProperties && andActionSQL.FilterProperties.Count > 0)
                                {
                                    Dictionary<string, object>.Enumerator e = andActionSQL.FilterProperties.GetEnumerator();
                                    while (e.MoveNext())
                                    {
                                        if (!result.FilterProperties.ContainsKey(e.Current.Key))
                                        {
                                            result.FilterProperties.Add(e.Current.Key, e.Current.Value);
                                        }
                                    }
                                }
                            }
                            else if (item.Value is String)
                            {
                                if (!string.IsNullOrWhiteSpace(result.SQL))
                                {
                                    result.SQL += " AND ";
                                }
                                result.SQL += item.Value;
                            }
                        }
                        break;
                    case FilterOperator.Or:
                        if (item.Value != null && !string.IsNullOrWhiteSpace(item.Value.ToString()))
                        {
                            if (item.Value is SubFilter)
                            {
                                if (!string.IsNullOrWhiteSpace(result.SQL))
                                {
                                    result.SQL += " OR ";
                                }
                                CommandSQL orActionSQL = ((SubFilter)item.Value).Build(model);
                                result.SQL += orActionSQL.SQL;
                                result.Params.AddRange(orActionSQL.Params);
                                result.ForeignTables.AddRange(orActionSQL.ForeignTables);
                            }
                            else if (item.Value is String)
                            {
                                if (!string.IsNullOrWhiteSpace(result.SQL))
                                {
                                    result.SQL += " OR ";
                                }
                                result.SQL += item.Value;
                            }
                        }
                        break;
                    case FilterOperator.Equals:
                    case FilterOperator.NotEquals:
                    case FilterOperator.Gt:
                    case FilterOperator.Gte:
                    case FilterOperator.Lt:
                    case FilterOperator.Lte:
                    case FilterOperator.Like:
                    case FilterOperator.NotLike:
                        exprValue = (Function)expr.Value;
                        if (exprValue is UNDECIDED)
                        {
                            exprValue = ((UNDECIDED)exprValue).Resolve(model);
                        }
                        CommandUtils.CheckFunctionForeignProperty(model, exprValue, result.ForeignTables);

                        BuildSimpleOperatorSQL(result, model, item.Key, exprKey, exprValue, recordEqualsProperties);

                        break;
                    case FilterOperator.IsNull:
                    case FilterOperator.IsNotNull:
                        result.SQL += string.Concat(exprKey.Convert2SQL(model), _operatorSymbols[item.Key]);
                        break;
                    case FilterOperator.Between:
                        object[] values = (object[])expr.Value;

                        exprValue = (Function)values[0];
                        if (exprValue is UNDECIDED)
                        {
                            exprValue = ((UNDECIDED)exprValue).Resolve(model);
                        }
                        CommandUtils.CheckFunctionForeignProperty(model, exprValue, result.ForeignTables);

                        exprValue2 = (Function)values[1];
                        if (exprValue2 is UNDECIDED)
                        {
                            exprValue2 = ((UNDECIDED)exprValue2).Resolve(model);
                        }
                        CommandUtils.CheckFunctionForeignProperty(model, exprValue2, result.ForeignTables);

                        BuildBetweenOperatorSQL(result, model, exprKey, exprValue, exprValue2);

                        break;
                    case FilterOperator.In:
                    case FilterOperator.NotIn:
                        object[] inValues = (object[])expr.Value;
                        for (int i = 0; i < inValues.Length; i++)
                        {
                            if (inValues[i] is UNDECIDED)
                            {
                                inValues[i] = ((UNDECIDED)inValues[i]).Resolve(model);
                            }

                            if (inValues[i] is Function)
                            {
                                CommandUtils.CheckFunctionForeignProperty(model, (Function)inValues[i], result.ForeignTables);
                            }
                        }

                        BuildInOperatorSQL(result, model, item.Key, exprKey, inValues);

                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(result.SQL))
            {
                if (result.SQL.Contains(" OR ", StringComparison.OrdinalIgnoreCase) ||
                    result.SQL.Contains(" AND ", StringComparison.OrdinalIgnoreCase))
                {
                    if (!(result.SQL.StartsWith("(") && result.SQL.EndsWith(")")))
                    {
                        result.SQL = string.Concat("(", result.SQL, ")");
                    }
                }
            }

            return result;
        }

    }
}
