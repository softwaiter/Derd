using CodeM.Common.DbHelper;
using CodeM.Common.Tools.Json;
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
            CommandSQL result = new CommandSQL();

            Json2DynamicParser j2d = new Json2DynamicParser();

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
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                expr.Value, p.FieldType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                p.DoBeforeSaveProcessor(inputObj), p.FieldType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat("`", p.Owner.Table, "`.`", p.Field, "`=?");
                        break;
                    case FilterOperator.NotEquals:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                expr.Value, p.FieldType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                p.DoBeforeSaveProcessor(inputObj), p.FieldType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat("`", p.Owner.Table, "`.`", p.Field, "`<>?");
                        break;
                    case FilterOperator.Gt:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                expr.Value, p.FieldType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                p.DoBeforeSaveProcessor(inputObj), p.FieldType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat("`", p.Owner.Table, "`.`", p.Field, "`>?");
                        break;
                    case FilterOperator.Gte:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                expr.Value, p.FieldType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                p.DoBeforeSaveProcessor(inputObj), p.FieldType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat("`", p.Owner.Table, "`.`", p.Field, "`>=?");
                        break;
                    case FilterOperator.Lt:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                expr.Value, p.FieldType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                p.DoBeforeSaveProcessor(inputObj), p.FieldType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat("`", p.Owner.Table, "`.`", p.Field, "`<?");
                        break;
                    case FilterOperator.Lte:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                expr.Value, p.FieldType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                p.DoBeforeSaveProcessor(inputObj), p.FieldType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat("`", p.Owner.Table, "`.`", p.Field, "`<=?");
                        break;
                    case FilterOperator.Like:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                expr.Value, DbType.String, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                p.DoBeforeSaveProcessor(inputObj), DbType.String, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat("`", p.Owner.Table, "`.`", p.Field, "` LIKE ?");
                        break;
                    case FilterOperator.NotLike:
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                expr.Value, DbType.String, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = expr.Value;
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                p.DoBeforeSaveProcessor(inputObj), DbType.String, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.SQL += string.Concat("`", p.Owner.Table, "`.`", p.Field, "` NOT LIKE ?");
                        break;
                    case FilterOperator.IsNull:
                        result.SQL += string.Concat("`", p.Owner.Table, "`.`", p.Field, "` IS NULL");
                        break;
                    case FilterOperator.IsNotNull:
                        result.SQL += string.Concat("`", p.Owner.Table, "`.`", p.Field, "` IS NOT NULL");
                        break;
                    case FilterOperator.Between:
                        object[] values = (object[])expr.Value;
                        if (!p.NeedCalcBeforeSave)
                        {
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                values[0], p.FieldType, ParameterDirection.Input);
                            dp2 = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                values[1], p.FieldType, ParameterDirection.Input);
                        }
                        else
                        {
                            dynamic inputObj = currM.NewObject();
                            inputObj[p.Name] = values[0];
                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                p.DoBeforeSaveProcessor(inputObj), p.FieldType, ParameterDirection.Input);

                            dynamic inputObj2 = currM.NewObject();
                            inputObj2[p.Name] = values[1];
                            dp2 = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                p.DoBeforeSaveProcessor(inputObj2), p.FieldType, ParameterDirection.Input);
                        }
                        result.Params.Add(dp);
                        result.Params.Add(dp2);
                        result.SQL += string.Concat("`", p.Owner.Table, "`.`", p.Field, "` BETWEEN ? AND ?");
                        break;
                    case FilterOperator.In:
                        StringBuilder sbInSQL = new StringBuilder(string.Concat("`", p.Owner.Table, "`.`", p.Field, "` IN("));
                        object[] items = (object[])expr.Value;
                        for (int i = 0; i < items.Length; i++)
                        {
                            if (i > 0)
                            {
                                sbInSQL.Append(",");
                            }

                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                items[i], p.FieldType, ParameterDirection.Input);
                            result.Params.Add(dp);
                            sbInSQL.Append("?");
                        }
                        sbInSQL.Append(")");
                        result.SQL += sbInSQL.ToString();
                        break;
                    case FilterOperator.NotIn:
                        StringBuilder sbNotInSQL = new StringBuilder(string.Concat("`", p.Owner.Table, "`.`", p.Field, "` NOT IN("));
                        object[] notItems = (object[])expr.Value;
                        for (int i = 0; i < notItems.Length; i++)
                        {
                            if (i > 0)
                            {
                                sbNotInSQL.Append(",");
                            }

                            dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                                notItems[i], p.FieldType, ParameterDirection.Input);
                            result.Params.Add(dp);
                            sbNotInSQL.Append("?");
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
