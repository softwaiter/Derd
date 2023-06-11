using CodeM.Common.DbHelper;
using CodeM.Common.Orm.Dialect;
using CodeM.Common.Tools.DynamicObject;
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

        internal bool TryGetProperty(Model m, object value, out Property p)
        {
            p = null;

            if (value == null)
            {
                return false;
            }

            if (value is Function)
            {
                Function func = (Function)value;
                while (func.ChildFunction != null)
                {
                    func = func.ChildFunction;
                }
                value = func.PropertyName;
            }

            string propName = value.ToString();
            if (!propName.Contains("."))
            {
                if (m.HasProperty(propName))
                {
                    p = m.GetProperty(propName);
                    return true;
                }
            }
            else
            {
                Model currM = m;
                string[] subNames = propName.Split(".");
                for (int i = 0; i < subNames.Length; i++)
                {
                    string subName = subNames[i];
                    if (TryGetProperty(currM, subName, out p))
                    {
                        if (i < subNames.Length - 1)
                        {
                            Model subM = ModelUtils.GetModel(p.TypeValue);
                            if (subM != null)
                            {
                                currM = subM;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        public IFilter Equals(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Equals,
                new KeyValuePair<Function, object>(new NONE(name), value)));
            return this;
        }

        public IFilter Equals(Function function, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Equals,
                new KeyValuePair<Function, object>(function, value)));
            return this;
        }

        public IFilter NotEquals(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotEquals,
                new KeyValuePair<Function, object>(new NONE(name), value)));
            return this;
        }

        public IFilter NotEquals(Function function, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotEquals,
                new KeyValuePair<Function, object>(function, value)));
            return this;
        }

        public IFilter Gt(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gt,
                new KeyValuePair<Function, object>(new NONE(name), value)));
            return this;
        }

        public IFilter Gt(Function function, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gt,
                new KeyValuePair<Function, object>(function, value)));
            return this;
        }

        public IFilter Gte(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gte,
                new KeyValuePair<Function, object>(new NONE(name), value)));
            return this;
        }

        public IFilter Gte(Function function, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gte,
                new KeyValuePair<Function, object>(function, value)));
            return this;
        }

        public IFilter Lt(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lt,
                new KeyValuePair<Function, object>(new NONE(name), value)));
            return this;
        }

        public IFilter Lt(Function function, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lt,
                new KeyValuePair<Function, object>(function, value)));
            return this;
        }

        public IFilter Lte(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lte,
                new KeyValuePair<Function, object>(new NONE(name), value)));
            return this;
        }

        public IFilter Lte(Function function, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lte,
                new KeyValuePair<Function, object>(function, value)));
            return this;
        }

        public IFilter Like(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Like,
                new KeyValuePair<Function, object>(new NONE(name), value)));
            return this;
        }

        public IFilter Like(Function function, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Like,
                new KeyValuePair<Function, object>(function, value)));
            return this;
        }

        public IFilter NotLike(string name, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotLike,
                new KeyValuePair<Function, object>(new NONE(name), value)));
            return this;
        }

        public IFilter NotLike(Function function, object value)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotLike,
                new KeyValuePair<Function, object>(function, value)));
            return this;
        }

        public IFilter IsNull(string name)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.IsNull,
                new KeyValuePair<Function, object>(new NONE(name), true)));
            return this;
        }

        public IFilter IsNull(Function function)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.IsNull,
                new KeyValuePair<Function, object>(function, true)));
            return this;
        }

        public IFilter IsNotNull(string name)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.IsNotNull,
                new KeyValuePair<Function, object>(new NONE(name), true)));
            return this;
        }

        public IFilter IsNotNull(Function function)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.IsNotNull,
                new KeyValuePair<Function, object>(function, true)));
            return this;
        }

        public IFilter Between(string name, object value, object value2)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Between,
                new KeyValuePair<Function, object>(new NONE(name), new object[] { value, value2 })));
            return this;
        }

        public IFilter Between(Function function, object value, object value2)
        {
            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Between,
                new KeyValuePair<Function, object>(function, new object[] { value, value2 })));
            return this;
        }

        public IFilter In(string name, params object[] values)
        {
            if (values.Length == 1)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Equals,
                    new KeyValuePair<Function, object>(new NONE(name), values[0])));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.In,
                    new KeyValuePair<Function, object>(new NONE(name), values)));
            }
            return this;
        }

        public IFilter In(Function function, params object[] values)
        {
            if (values.Length == 1)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Equals,
                    new KeyValuePair<Function, object>(function, values[0])));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.In,
                    new KeyValuePair<Function, object>(function, values)));
            }
            return this;
        }

        public IFilter NotIn(string name, params object[] values)
        {
            if (values.Length == 1)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotEquals,
                    new KeyValuePair<Function, object>(new NONE(name), values[0])));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotIn,
                    new KeyValuePair<Function, object>(new NONE(name), values)));
            }
            return this;
        }

        public IFilter NotIn(Function function, params object[] values)
        {
            if (values.Length == 1)
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotEquals,
                    new KeyValuePair<Function, object>(function, values[0])));
            }
            else
            {
                mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotIn,
                    new KeyValuePair<Function, object>(function, values)));
            }
            return this;
        }

        private DbType GetParamDbType(Function function, Property p)
        {
            if (function != null && !(function is NONE))
            {
                string funcName = function.GetType().Name.ToUpper();
                string funcType = Features.GetFunctionReturnType(p.Owner, funcName);
                if (string.IsNullOrWhiteSpace(funcType))
                {
                    return CommandUtils.GetDbParamType(p);
                }
                else
                {
                    return (DbType)Enum.Parse(typeof(DbType), funcType, true);
                }
            }
            else
            {
                return CommandUtils.GetDbParamType(p);
            }
        }

        private bool IsFunctionExpr(object obj)
        {
            if (obj != null)
            {
                return (obj is Function && !(obj is NONE));
            }
            return false;
        }

        internal CommandSQL Build(Model model)
        {
            string[] quotes = Features.GetObjectQuotes(model);

            bool recordEqualsProperties = true;
            CommandSQL result = new CommandSQL();

            Model currM;
            KeyValuePair<Function, object> expr, defaultExpr = new KeyValuePair<Function, object>();
            bool valueIsProp;
            Property p = null, p2 = null, p3 = null;
            DbParameter dp, dp2;
            string exprLeft = null, exprRight = null;
            foreach (KeyValuePair<FilterOperator, object> item in mFilterItems)
            {
                currM = model;
                expr = defaultExpr;
                valueIsProp = false;

                if (item.Key != FilterOperator.And &&
                    item.Key != FilterOperator.Or)
                {
                    expr = (KeyValuePair<Function, object>)item.Value;
                    if (expr.Key is NONE)
                    {
                        PropertyChecker.CheckValueProperty(model, expr.Key.PropertyName);
                    }
                    else
                    {
                        PropertyChecker.CheckFunctionProperty(model, expr.Key);
                    }

                    TryGetProperty(model, expr.Key, out p);
                    valueIsProp = TryGetProperty(model, expr.Value, out p2);

                    // 将带.分隔符的搜索条件添加到关联表变量中
                    if (expr.Key.PropertyName.Contains("."))
                    {
                        result.ForeignTables.Add(expr.Key.PropertyName);
                    }

                    if (!valueIsProp)
                    {
                        // 对当前模型直接属性Equals的表达式，转换成对象属性值
                        if (!IsFunctionExpr(expr.Value) && !expr.Key.PropertyName.Contains("."))
                        {
                            if (recordEqualsProperties && item.Key == FilterOperator.Equals)
                            {
                                if (!result.FilterProperties.ContainsKey(p.Name))
                                {
                                    result.FilterProperties.Add(p.Name, expr.Value);
                                }
                            }
                        }
                    }
                    else
                    {
                        // 如果条件表达式值为属性类型，且属性非当前模型直接属性，需加入关联表变量中
                        string propName = expr.Value.ToString();
                        if (expr.Value is Function)
                        {
                            propName = ((Function)expr.Value).PropertyName;
                        }

                        if (propName.Contains("."))
                        {
                            result.ForeignTables.Add(propName);
                        }
                    }

                    if (!string.IsNullOrEmpty(result.SQL))
                    {
                        result.SQL += " AND ";
                    }

                    exprLeft = SQLBuilder.GenFunctionSQL(currM, expr.Key,
                        string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1]));
                }
                else if (item.Key == FilterOperator.Or)
                {
                    recordEqualsProperties = false;
                    result.FilterProperties.Clear();
                }

                DbType dbType = DbType.String;
                string paramName = null;
                string paramPlaceholder = null;
                string paramName2 = null;
                string paramPlaceholder2 = null;
                if (p != null)
                {
                    dbType = GetParamDbType(expr.Key, p);
                    paramName = CommandUtils.GenParamName(p);
                    paramPlaceholder = Features.GetCommandParamName(currM, paramName);
                    paramName2 = CommandUtils.GenParamName(p);
                    paramPlaceholder2 = Features.GetCommandParamName(currM, paramName2);
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
                        if (!valueIsProp && !IsFunctionExpr(expr.Value))
                        {
                            if (!p.NeedCalcPreSaveProcessor)
                            {
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    expr.Value, dbType, ParameterDirection.Input);
                            }
                            else
                            {
                                dynamic inputObj = new DynamicObjectExt();
                                inputObj.SetValue(p.Name, expr.Value);
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    p.DoPreSaveProcessor(inputObj), dbType, ParameterDirection.Input);
                            }
                            result.Params.Add(dp);

                            result.SQL += string.Concat(exprLeft, "=", paramPlaceholder);
                        }
                        else
                        {
                            if (p2 != null)
                            {
                                exprRight = string.Concat(quotes[0], p2.Owner.Table, quotes[1], ".", quotes[0], p2.Field, quotes[1]);
                            }
                            else
                            {
                                exprRight = string.Concat("'", ((Function)expr.Value).PropertyName, "'");
                            }

                            if (expr.Value is Function)
                            {
                                exprRight = SQLBuilder.GenFunctionSQL(currM, (Function)expr.Value, exprRight);
                            }
                            result.SQL += string.Concat(exprLeft, "=", exprRight);
                        }
                        break;
                    case FilterOperator.NotEquals:
                        if (!valueIsProp && !IsFunctionExpr(expr.Value))
                        {
                            if (!p.NeedCalcPreSaveProcessor)
                            {
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    expr.Value, dbType, ParameterDirection.Input);
                            }
                            else
                            {
                                dynamic inputObj = new DynamicObjectExt();
                                inputObj.SetValue(p.Name, expr.Value);
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    p.DoPreSaveProcessor(inputObj), dbType, ParameterDirection.Input);
                            }
                            result.Params.Add(dp);
                            result.SQL += string.Concat(exprLeft, "<>", paramPlaceholder);
                        }
                        else
                        {
                            if (p2 != null)
                            {
                                exprRight = string.Concat(quotes[0], p2.Owner.Table, quotes[1], ".", quotes[0], p2.Field, quotes[1]);
                            }
                            else
                            {
                                exprRight = string.Concat("'", ((Function)expr.Value).PropertyName, "'");
                            }

                            if (expr.Value is Function)
                            {
                                exprRight = SQLBuilder.GenFunctionSQL(currM, (Function)expr.Value, exprRight);
                            }
                            result.SQL += string.Concat(exprLeft, "<>", exprRight);
                        }
                        break;
                    case FilterOperator.Gt:
                        if (!valueIsProp && !IsFunctionExpr(expr.Value))
                        {
                            if (!p.NeedCalcPreSaveProcessor)
                            {
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    expr.Value, dbType, ParameterDirection.Input);
                            }
                            else
                            {
                                dynamic inputObj = new DynamicObjectExt();
                                inputObj.SetValue(p.Name, expr.Value);
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    p.DoPreSaveProcessor(inputObj), dbType, ParameterDirection.Input);
                            }
                            result.Params.Add(dp);
                            result.SQL += string.Concat(exprLeft, ">", paramPlaceholder);
                        }
                        else
                        {
                            if (p2 != null)
                            {
                                exprRight = string.Concat(quotes[0], p2.Owner.Table, quotes[1], ".", quotes[0], p2.Field, quotes[1]);
                            }
                            else
                            {
                                exprRight = string.Concat("'", ((Function)expr.Value).PropertyName, "'");
                            }

                            if (expr.Value is Function)
                            {
                                exprRight = SQLBuilder.GenFunctionSQL(currM, (Function)expr.Value, exprRight);
                            }
                            result.SQL += string.Concat(exprLeft, ">", exprRight);
                        }
                        break;
                    case FilterOperator.Gte:
                        if (!valueIsProp && !IsFunctionExpr(expr.Value))
                        {
                            if (!p.NeedCalcPreSaveProcessor)
                            {
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    expr.Value, dbType, ParameterDirection.Input);
                            }
                            else
                            {
                                dynamic inputObj = new DynamicObjectExt();
                                inputObj.SetValue(p.Name, expr.Value);
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    p.DoPreSaveProcessor(inputObj), dbType, ParameterDirection.Input);
                            }
                            result.Params.Add(dp);
                            result.SQL += string.Concat(exprLeft, ">=", paramPlaceholder);
                        }
                        else
                        {
                            if (p2 != null)
                            {
                                exprRight = string.Concat(quotes[0], p2.Owner.Table, quotes[1], ".", quotes[0], p2.Field, quotes[1]);
                            }
                            else
                            {
                                exprRight = string.Concat("'", ((Function)expr.Value).PropertyName, "'");
                            }

                            if (expr.Value is Function)
                            {
                                exprRight = SQLBuilder.GenFunctionSQL(currM, (Function)expr.Value, exprRight);
                            }
                            result.SQL += string.Concat(exprLeft, ">=", exprRight);
                        }
                        break;
                    case FilterOperator.Lt:
                        if (!valueIsProp && !IsFunctionExpr(expr.Value))
                        {
                            if (!p.NeedCalcPreSaveProcessor)
                            {
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    expr.Value, dbType, ParameterDirection.Input);
                            }
                            else
                            {
                                dynamic inputObj = new DynamicObjectExt();
                                inputObj.SetValue(p.Name, expr.Value);
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    p.DoPreSaveProcessor(inputObj), dbType, ParameterDirection.Input);
                            }
                            result.Params.Add(dp);
                            result.SQL += string.Concat(exprLeft, "<", paramPlaceholder);
                        }
                        else 
                        {
                            if (p2 != null)
                            {
                                exprRight = string.Concat(quotes[0], p2.Owner.Table, quotes[1], ".", quotes[0], p2.Field, quotes[1]);
                            }
                            else
                            {
                                exprRight = string.Concat("'", ((Function)expr.Value).PropertyName, "'");
                            }

                            if (expr.Value is Function)
                            {
                                exprRight = SQLBuilder.GenFunctionSQL(currM, (Function)expr.Value, exprRight);
                            }
                            result.SQL += string.Concat(exprLeft, "<", exprRight);
                        }
                        break;
                    case FilterOperator.Lte:
                        if (!valueIsProp && !IsFunctionExpr(expr.Value))
                        {
                            if (!p.NeedCalcPreSaveProcessor)
                            {
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    expr.Value, dbType, ParameterDirection.Input);
                            }
                            else
                            {
                                dynamic inputObj = new DynamicObjectExt();
                                inputObj.SetValue(p.Name, expr.Value);
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    p.DoPreSaveProcessor(inputObj), dbType, ParameterDirection.Input);
                            }
                            result.Params.Add(dp);
                            result.SQL += string.Concat(exprLeft, "<=", paramPlaceholder);
                        }
                        else
                        {
                            if (p2 != null)
                            {
                                exprRight = string.Concat(quotes[0], p2.Owner.Table, quotes[1], ".", quotes[0], p2.Field, quotes[1]);
                            }
                            else
                            {
                                exprRight = string.Concat("'", ((Function)expr.Value).PropertyName, "'");
                            }

                            if (expr.Value is Function)
                            {
                                exprRight = SQLBuilder.GenFunctionSQL(currM, (Function)expr.Value, exprRight);
                            }
                            result.SQL += string.Concat(exprLeft, "<=", exprRight);
                        }
                        break;
                    case FilterOperator.Like:
                        if (!valueIsProp && !IsFunctionExpr(expr.Value))
                        {
                            if (!p.NeedCalcPreSaveProcessor)
                            {
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    expr.Value, DbType.String, ParameterDirection.Input);
                            }
                            else
                            {
                                dynamic inputObj = new DynamicObjectExt();
                                inputObj.SetValue(p.Name, expr.Value);
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    p.DoPreSaveProcessor(inputObj), DbType.String, ParameterDirection.Input);
                            }
                            result.Params.Add(dp);
                            result.SQL += string.Concat(exprLeft, " LIKE ", paramPlaceholder);
                        }
                        else
                        {
                            if (p2 != null)
                            {
                                exprRight = string.Concat(quotes[0], p2.Owner.Table, quotes[1], ".", quotes[0], p2.Field, quotes[1]);
                            }
                            else
                            {
                                exprRight = string.Concat("'", ((Function)expr.Value).PropertyName, "'");
                            }

                            if (expr.Value is Function)
                            {
                                exprRight = SQLBuilder.GenFunctionSQL(currM, (Function)expr.Value, exprRight);
                            }
                            result.SQL += string.Concat(exprLeft, " LIKE ", exprRight);
                        }
                        break;
                    case FilterOperator.NotLike:
                        if (!valueIsProp && !IsFunctionExpr(expr.Value))
                        {
                            if (!p.NeedCalcPreSaveProcessor)
                            {
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    expr.Value, DbType.String, ParameterDirection.Input);
                            }
                            else
                            {
                                dynamic inputObj = new DynamicObjectExt();
                                inputObj.SetValue(p.Name, expr.Value);
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    p.DoPreSaveProcessor(inputObj), DbType.String, ParameterDirection.Input);
                            }
                            result.Params.Add(dp);
                            result.SQL += string.Concat(exprLeft, " NOT LIKE ", paramPlaceholder);
                        }
                        else
                        {
                            if (p2 != null)
                            {
                                exprRight = string.Concat(quotes[0], p2.Owner.Table, quotes[1], ".", quotes[0], p2.Field, quotes[1]);
                            }
                            else
                            {
                                exprRight = string.Concat("'", ((Function)expr.Value).PropertyName, "'");
                            }

                            if (expr.Value is Function)
                            {
                                exprRight = SQLBuilder.GenFunctionSQL(currM, (Function)expr.Value, exprRight);
                            }
                            result.SQL += string.Concat(exprLeft, " NOT LIKE ", exprRight);
                        }
                        break;
                    case FilterOperator.IsNull:
                        result.SQL += string.Concat(exprLeft, " IS NULL");
                        break;
                    case FilterOperator.IsNotNull:
                        result.SQL += string.Concat(exprLeft, " IS NOT NULL");
                        break;
                    case FilterOperator.Between:
                        object[] values = (object[])expr.Value;

                        bool value1IsProp = TryGetProperty(model, values[0], out p2);
                        bool value2IsProp = TryGetProperty(model, values[1], out p3);

                        if (!p.NeedCalcPreSaveProcessor)
                        {
                            if (!value1IsProp && !IsFunctionExpr(values[0]))
                            {
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    values[0], dbType, ParameterDirection.Input);
                                result.Params.Add(dp);
                            }

                            if (!value2IsProp && !IsFunctionExpr(values[1]))
                            {
                                dp2 = DbUtils.CreateParam(currM.Path, paramName2,
                                    values[1], dbType, ParameterDirection.Input);
                                result.Params.Add(dp2);
                            }
                        }
                        else
                        {
                            if (!value1IsProp && !IsFunctionExpr(values[0]))
                            {
                                dynamic inputObj = new DynamicObjectExt();
                                inputObj.SetValue(p.Name, values[0]);
                                dp = DbUtils.CreateParam(currM.Path, paramName,
                                    p.DoPreSaveProcessor(inputObj), dbType, ParameterDirection.Input);
                                result.Params.Add(dp);
                            }

                            if (!value2IsProp && !IsFunctionExpr(values[1]))
                            {
                                dynamic inputObj2 = new DynamicObjectExt();
                                inputObj2.SetValue(p.Name, values[1]);
                                dp2 = DbUtils.CreateParam(currM.Path, paramName2,
                                    p.DoPreSaveProcessor(inputObj2), dbType, ParameterDirection.Input);
                                result.Params.Add(dp2);
                            }
                        }

                        string exprRight1 = paramPlaceholder;
                        string exprRight2 = paramPlaceholder2;
                        if (value1IsProp || IsFunctionExpr(values[0]))
                        {
                            if (p2 != null)
                            {
                                exprRight1 = string.Concat(quotes[0], p2.Owner.Table, quotes[1], ".", quotes[0], p2.Field, quotes[1]);
                            }
                            else
                            {
                                exprRight1 = string.Concat("'", ((Function)values[0]).PropertyName, "'");
                            }

                            if (values[0] is Function)
                            {
                                exprRight1 = SQLBuilder.GenFunctionSQL(currM, (Function)values[0], exprRight1);
                            }
                        }
                        if (value2IsProp || IsFunctionExpr(values[1]))
                        {
                            if (p3 != null)
                            {
                                exprRight2 = string.Concat(quotes[0], p3.Owner.Table, quotes[1], ".", quotes[0], p3.Field, quotes[1]);
                            }
                            else
                            {
                                exprRight2 = string.Concat("'", ((Function)values[1]).PropertyName, "'");
                            }

                            if (values[1] is Function)
                            {
                                exprRight2 = SQLBuilder.GenFunctionSQL(currM, (Function)values[1], exprRight2);
                            }
                        }

                        result.SQL += string.Concat(exprLeft, " BETWEEN ", exprRight1, " AND ", exprRight2);
                        break;
                    case FilterOperator.In:
                        StringBuilder sbInSQL = new StringBuilder(string.Concat(exprLeft, " IN("));
                        object[] items = (object[])expr.Value;
                        for (int i = 0; i < items.Length; i++)
                        {
                            if (i > 0)
                            {
                                sbInSQL.Append(",");
                            }

                            valueIsProp = TryGetProperty(model, items[i], out p2);
                            if (!valueIsProp && !IsFunctionExpr(items[i]))
                            {
                                string inParamName = CommandUtils.GenParamName(p);
                                dp = DbUtils.CreateParam(currM.Path, inParamName,
                                    items[i], dbType, ParameterDirection.Input);
                                result.Params.Add(dp);
                                string inParamPlaceholder = Features.GetCommandParamName(currM, inParamName);
                                sbInSQL.Append(inParamPlaceholder);
                            }
                            else
                            {
                                if (p2 != null)
                                {
                                    exprRight = string.Concat(quotes[0], p2.Owner.Table, quotes[1], ".", quotes[0], p2.Field, quotes[1]);
                                }
                                else
                                {
                                    exprRight = string.Concat("'", ((Function)items[i]).PropertyName, "'");
                                }

                                if (items[i] is Function)
                                {
                                    exprRight = SQLBuilder.GenFunctionSQL(currM, (Function)items[i], exprRight);
                                }

                                sbInSQL.Append(exprRight);
                            }
                        }
                        sbInSQL.Append(")");
                        result.SQL += sbInSQL.ToString();
                        break;
                    case FilterOperator.NotIn:
                        StringBuilder sbNotInSQL = new StringBuilder(string.Concat(exprLeft, " NOT IN("));
                        object[] notItems = (object[])expr.Value;
                        for (int i = 0; i < notItems.Length; i++)
                        {
                            if (i > 0)
                            {
                                sbNotInSQL.Append(",");
                            }

                            valueIsProp = TryGetProperty(model, notItems[i], out p2);
                            if (!valueIsProp && !IsFunctionExpr(notItems[i]))
                            {
                                string notInParamName = CommandUtils.GenParamName(p);
                                dp = DbUtils.CreateParam(currM.Path, notInParamName,
                                    notItems[i], dbType, ParameterDirection.Input);
                                result.Params.Add(dp);
                                string notInParamPlaceholder = Features.GetCommandParamName(currM, notInParamName);
                                sbNotInSQL.Append(notInParamPlaceholder);
                            }
                            else
                            {
                                if (p2 != null)
                                {
                                    exprRight = string.Concat(quotes[0], p2.Owner.Table, quotes[1], ".", quotes[0], p2.Field, quotes[1]);
                                }
                                else
                                {
                                    exprRight = string.Concat("'", ((Function)notItems[i]).PropertyName, "'");
                                }

                                if (notItems[i] is Function)
                                {
                                    exprRight = SQLBuilder.GenFunctionSQL(currM, (Function)notItems[i], exprRight);
                                }

                                sbNotInSQL.Append(exprRight);
                            }
                        }
                        sbNotInSQL.Append(")");
                        result.SQL += sbNotInSQL.ToString();
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
