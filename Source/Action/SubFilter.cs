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
        private Model mModel;
        private IFilter mParent = null;
        private List<KeyValuePair<FilterOperator, object>> mFilterItems = new List<KeyValuePair<FilterOperator, object>>();

        public SubFilter(Model m)
        {
            mModel = m;
        }

        public Model Owner
        {
            get
            {
                return mModel;
            }
        }

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

            if (value == null ||
                value.GetType() != typeof(string))
            {
                return false;
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

        internal void CheckProperty(string name)
        {
            if (!TryGetProperty(Owner, name, out Property p))
            {
                throw new Exception("无法识别的属性：" + name);
            }
        }

        public IFilter Equals(string name, object value)
        {
            CheckProperty(name);

            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Equals,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter NotEquals(string name, object value)
        {
            CheckProperty(name);

            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotEquals,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter Gt(string name, object value)
        {
            CheckProperty(name);

            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gt,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter Gte(string name, object value)
        {
            CheckProperty(name);

            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Gte,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter Lt(string name, object value)
        {
            CheckProperty(name);

            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lt,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter Lte(string name, object value)
        {
            CheckProperty(name);

            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Lte,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter Like(string name, string value)
        {
            CheckProperty(name);

            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Like,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter NotLike(string name, string value)
        {
            CheckProperty(name);

            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.NotLike,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public IFilter IsNull(string name)
        {
            CheckProperty(name);

            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.IsNull,
                new KeyValuePair<string, object>(name, true)));
            return this;
        }

        public IFilter IsNotNull(string name)
        {
            CheckProperty(name);

            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.IsNotNull,
                new KeyValuePair<string, object>(name, true)));
            return this;
        }

        public IFilter Between(string name, object value, object value2)
        {
            CheckProperty(name);

            mFilterItems.Add(new KeyValuePair<FilterOperator, object>(FilterOperator.Between,
                new KeyValuePair<string, object>(name, new object[] { value, value2 })));
            return this;
        }

        public IFilter In(string name, params object[] values)
        {
            CheckProperty(name);

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
            CheckProperty(name);

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

            bool recordEqualsProperties = true;
            CommandSQL result = new CommandSQL();

            KeyValuePair<string, object> expr = new KeyValuePair<string, object>();
            Model currM;
            Property p = null, p2 = null;
            DbParameter dp, dp2;
            foreach (KeyValuePair<FilterOperator, object> item in mFilterItems)
            {
                currM = model;

                if (item.Key != FilterOperator.And &&
                    item.Key != FilterOperator.Or)
                {
                    expr = (KeyValuePair<string, object>)item.Value;

                    bool keyIsProp = TryGetProperty(model, expr.Key, out p);
                    bool valueIsProp = TryGetProperty(model, expr.Value, out p2);

                    if (valueIsProp)
                    {
                    }
                    else
                    {
                        if (!expr.Key.Contains("."))
                        {
                            if (recordEqualsProperties && item.Key == FilterOperator.Equals)
                            {
                                if (!result.FilterProperties.ContainsKey(p.Name))
                                {
                                    result.FilterProperties.Add(p.Name, expr.Value);
                                }
                            }
                        }
                        else
                        {
                            result.ForeignTables.Add(expr.Key);
                        }
                    }

                    if (!expr.Key.Contains("."))
                    {
                        p = model.GetProperty(expr.Key);
                        if (recordEqualsProperties && item.Key == FilterOperator.Equals)
                        {
                            if (!result.FilterProperties.ContainsKey(p.Name))
                            {
                                result.FilterProperties.Add(p.Name, expr.Value);
                            }
                        }
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
                    dbType = CommandUtils.GetDbParamType(p);
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
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], "=", paramPlaceholder);
                        break;
                    case FilterOperator.NotEquals:
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
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], "<>", paramPlaceholder);
                        break;
                    case FilterOperator.Gt:
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
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], ">", paramPlaceholder);
                        break;
                    case FilterOperator.Gte:
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
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], ">=", paramPlaceholder);
                        break;
                    case FilterOperator.Lt:
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
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], "<", paramPlaceholder);
                        break;
                    case FilterOperator.Lte:
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
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], "<=", paramPlaceholder);
                        break;
                    case FilterOperator.Like:
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
                        result.SQL += string.Concat(quotes[0], p.Owner.Table, quotes[1], ".", quotes[0], p.Field, quotes[1], " LIKE ", paramPlaceholder);
                        break;
                    case FilterOperator.NotLike:
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
                        if (!p.NeedCalcPreSaveProcessor)
                        {
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                values[0], dbType, ParameterDirection.Input);
                            dp2 = DbUtils.CreateParam(currM.Path, paramName2,
                                values[1], dbType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = new DynamicObjectExt();
                            inputObj.SetValue(p.Name, values[0]);
                            dp = DbUtils.CreateParam(currM.Path, paramName,
                                p.DoPreSaveProcessor(inputObj), dbType, ParameterDirection.Input);

                            dynamic inputObj2 = new DynamicObjectExt();
                            inputObj2.SetValue(p.Name, values[1]);
                            dp2 = DbUtils.CreateParam(currM.Path, paramName2,
                                p.DoPreSaveProcessor(inputObj2), dbType, ParameterDirection.Input);
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
