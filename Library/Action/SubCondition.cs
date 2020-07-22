using CodeM.Common.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace CodeM.Common.Orm
{
    public enum ConditionOperator
    {
        And = 1,
        Or = 2,
        Equals = 4,
        NotEquals = 8
    }

    public class SubCondition : ICondition
    {
        private List<KeyValuePair<ConditionOperator, object>> mConditions = new List<KeyValuePair<ConditionOperator, object>>();

        public void Reset()
        {
            mConditions.Clear();
        }

        public ICondition And(ICondition subCondition)
        {
            mConditions.Add(new KeyValuePair<ConditionOperator, object>(ConditionOperator.And, subCondition));
            return this;
        }

        public ICondition Or(ICondition subCondition)
        {
            mConditions.Add(new KeyValuePair<ConditionOperator, object>(ConditionOperator.Or, subCondition));
            return this;
        }

        public ICondition Equals(string name, object value)
        {
            mConditions.Add(new KeyValuePair<ConditionOperator, object>(ConditionOperator.Equals,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        public ICondition NotEquals(string name, object value)
        {
            mConditions.Add(new KeyValuePair<ConditionOperator, object>(ConditionOperator.NotEquals,
                new KeyValuePair<string, object>(name, value)));
            return this;
        }

        internal CommandSQL Build(Model model)
        {
            CommandSQL result = new CommandSQL();

            foreach (KeyValuePair<ConditionOperator, object> item in mConditions)
            {
                if (!string.IsNullOrEmpty(result.SQL) &&
                    item.Key != ConditionOperator.And &&
                    item.Key != ConditionOperator.Or)
                {
                    result.SQL += " AND ";
                }

                switch (item.Key)
                {
                    case ConditionOperator.And:
                        CommandSQL andActionSQL = ((SubCondition)item.Value).Build(model);
                        result.SQL += string.Concat(" AND ", andActionSQL.SQL);
                        result.Params.AddRange(andActionSQL.Params);
                        break;
                    case ConditionOperator.Or:
                        CommandSQL orActionSQL = ((SubCondition)item.Value).Build(model);
                        result.SQL += string.Concat(" OR ", orActionSQL.SQL);
                        result.Params.AddRange(orActionSQL.Params);
                        break;
                    case ConditionOperator.Equals:
                        KeyValuePair<string, object> expr = (KeyValuePair<string, object>)item.Value;

                        Property p = model.GetProperty(expr.Key);
                        if (p == null)
                        {
                            throw new Exception(string.Concat("无效的属性：", expr.Key));
                        }

                        DbParameter dp = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr.Value, p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.SQL += string.Concat(p.Field, "=?");
                        break;
                    case ConditionOperator.NotEquals:
                        KeyValuePair<string, object> expr2 = (KeyValuePair<string, object>)item.Value;

                        Property p2 = model.GetProperty(expr2.Key);
                        if (p2 == null)
                        {
                            throw new Exception(string.Concat("无效的属性：", expr2.Key));
                        }

                        DbParameter dp2 = DbUtils.CreateParam(model.Path, Guid.NewGuid().ToString("N"),
                            expr2.Value, p2.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp2);
                        result.SQL += string.Concat(p2.Field, "<>?");
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
