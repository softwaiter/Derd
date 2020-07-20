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

        internal ActionSQL Build(Model model)
        {
            ActionSQL result = new ActionSQL();

            foreach (KeyValuePair<ConditionOperator, object> item in mConditions)
            {
                if (!string.IsNullOrEmpty(result.Command) &&
                    item.Key != ConditionOperator.And &&
                    item.Key != ConditionOperator.Or)
                {
                    result.Command += " AND ";
                }

                switch (item.Key)
                {
                    case ConditionOperator.And:
                        ActionSQL andActionSQL = ((SubCondition)item.Value).Build(model);
                        result.Command += string.Concat(" AND ", andActionSQL.Command);
                        result.Params.AddRange(andActionSQL.Params);
                        break;
                    case ConditionOperator.Or:
                        ActionSQL orActionSQL = ((SubCondition)item.Value).Build(model);
                        result.Command += string.Concat(" OR ", orActionSQL.Command);
                        result.Params.AddRange(orActionSQL.Params);
                        break;
                    case ConditionOperator.Equals:
                        KeyValuePair<string, object> expr = (KeyValuePair<string, object>)item.Value;

                        Property p = model.GetProperty(expr.Key);
                        if (p == null)
                        {
                            throw new Exception(string.Concat("无效的属性：", expr.Key));
                        }

                        DbParameter dp = DbUtils.CreateParam(model.Path, p.Name,
                            expr.Value, p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);
                        result.Command += string.Concat(p.Field, "=?");
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(result.Command))
            {
                result.Command = string.Concat("(", result.Command, ")");
            }

            return result;
        }

    }
}
