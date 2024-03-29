﻿using CodeM.Common.DbHelper;
using CodeM.Common.Orm.SQL;
using CodeM.Common.Orm.SQL.Dialect;
using CodeM.Common.Tools.DynamicObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace CodeM.Common.Orm
{
    internal class SQLBuilder
    {
        private static CommandSQL _BuildOracleBatchInsertSQL(Model modelDefine, List<dynamic> batchModelValues)
        {
            string[] quotes = Features.GetObjectQuotes(modelDefine);
            dynamic firstModelValues = batchModelValues[0];

            CommandSQL result = new CommandSQL();

            object value;
            string insertFields = string.Empty;
            List<Property> insertProperties = new List<Property>();
            for (int i = 0; i < modelDefine.PropertyCount; i++)
            {
                Property p = modelDefine.GetProperty(i);
                if (p.JoinInsert)
                {
                    if (modelDefine.TryGetValue(firstModelValues, p, out value))
                    {
                        if (value != null)
                        {
                            if (insertFields.Length > 0)
                            {
                                insertFields += ",";
                            }
                            insertFields += string.Concat(quotes[0], p.Field, quotes[1]);

                            insertProperties.Add(p);
                        }
                    }
                }
            }

            StringBuilder sbValues = new StringBuilder(batchModelValues.Count * 100);
            for (int i = 0; i < batchModelValues.Count; i++)
            {
                dynamic currModelValues = batchModelValues[i];

                if (sbValues.Length > 0)
                {
                    sbValues.Append(" UNION ");
                }

                sbValues.Append("SELECT ");
                for (int j = 0; j < insertProperties.Count; j++)
                {
                    Property p = insertProperties[j];
                    if (!modelDefine.TryGetValue(currModelValues, p, out value))
                    {
                        value = null;
                    }

                    if (value != null && value is Function)
                    {
                        if (j > 0)
                        {
                            sbValues.Append(",");
                        }
                        sbValues.Append(((Function)value).Convert2SQL(modelDefine));
                    }
                    else if (value != null && value is PropertyValue)
                    {
                        if (j > 0)
                        {
                            sbValues.Append(",");
                        }
                        sbValues.Append(((PropertyValue)value).Convert2SQL(p));
                    }
                    else
                    {
                        if (value != null)
                        {
                            if (p.RealType == typeof(DynamicObjectExt))
                            {
                                value = value.ToString();
                            }
                            else if (p.RealType == typeof(bool))
                            {
                                value = bool.Parse("" + value);
                                if (Features.IsUseIntegerInsteadOfBool(modelDefine))
                                {
                                    value = (bool)value ? 1 : 0;
                                }
                            }
                            else
                            {
                                Type fieldType = CommandUtils.DbType2Type(p.FieldType);
                                if (fieldType != value.GetType())
                                {
                                    value = Convert.ChangeType(value, fieldType);
                                }
                            }
                        }

                        string paramName = CommandUtils.GenParamName(p) + i;
                        DbType dbType = CommandUtils.GetDbParamType(p);
                        DbParameter dp = DbUtils.CreateParam(modelDefine.Path, paramName,
                            value, dbType, ParameterDirection.Input);
                        result.Params.Add(dp);

                        if (j > 0)
                        {
                            sbValues.Append(",");
                        }

                        string paramPlaceholder = Features.GetCommandParamName(modelDefine, paramName);
                        sbValues.Append(paramPlaceholder);
                    }
                }
                sbValues.Append(" FROM DUAL");
            }

            result.SQL = string.Concat("INSERT INTO ", quotes[0], modelDefine.Table, quotes[1], " (", insertFields, ") ", sbValues);
            sbValues.Clear();

            return result;
        }

        private static CommandSQL _BuildSqlserverBatchInsertSQL(Model modelDefine, List<dynamic> batchModelValues)
        {
            string[] quotes = Features.GetObjectQuotes(modelDefine);
            dynamic firstModelValues = batchModelValues[0];

            CommandSQL result = new CommandSQL();

            object value;
            string insertFields = string.Empty;
            List<Property> insertProperties = new List<Property>();
            for (int i = 0; i < modelDefine.PropertyCount; i++)
            {
                Property p = modelDefine.GetProperty(i);
                if (p.JoinInsert)
                {
                    if (modelDefine.TryGetValue(firstModelValues, p, out value))
                    {
                        if (value != null)
                        {
                            if (insertFields.Length > 0)
                            {
                                insertFields += ",";
                            }
                            insertFields += string.Concat(quotes[0], p.Field, quotes[1]);

                            insertProperties.Add(p);
                        }
                    }
                }
            }

            StringBuilder sbValues = new StringBuilder(batchModelValues.Count * 100);
            for (int i = 0; i < batchModelValues.Count; i++)
            {
                dynamic currModelValues = batchModelValues[i];

                if (sbValues.Length > 0)
                {
                    sbValues.Append(" UNION ");
                }

                sbValues.Append("SELECT ");
                for (int j = 0; j < insertProperties.Count; j++)
                {
                    Property p = insertProperties[j];
                    if (!modelDefine.TryGetValue(currModelValues, p, out value))
                    {
                        value = null;
                    }

                    if (value != null && value is Function)
                    {
                        if (j > 0)
                        {
                            sbValues.Append(",");
                        }

                        bool isNeedQuote = (p.FieldType == DbType.String ||
                            p.FieldType == DbType.StringFixedLength ||
                            p.FieldType == DbType.AnsiString ||
                            p.FieldType == DbType.AnsiStringFixedLength ||
                            p.FieldType == DbType.Date ||
                            p.FieldType == DbType.DateTime ||
                            p.FieldType == DbType.DateTime2);

                        string funcValue = ((Function)value).Convert2SQL(modelDefine);

                        if (isNeedQuote && !funcValue.StartsWith("'") && !funcValue.EndsWith("'"))
                        {
                            sbValues.Append("'").Append(funcValue).Append("'");
                        }
                        else
                        {
                            sbValues.Append(funcValue);
                        }
                    }
                    else if (value != null && value is PropertyValue)
                    {
                        if (j > 0)
                        {
                            sbValues.Append(",");
                        }

                        bool isNeedQuote = (p.FieldType == DbType.String ||
                            p.FieldType == DbType.StringFixedLength ||
                            p.FieldType == DbType.AnsiString ||
                            p.FieldType == DbType.AnsiStringFixedLength ||
                            p.FieldType == DbType.Date ||
                            p.FieldType == DbType.DateTime ||
                            p.FieldType == DbType.DateTime2);

                        string propValue = ((PropertyValue)value).Convert2SQL(p);

                        if (isNeedQuote && !propValue.StartsWith("'") && !propValue.EndsWith("'"))
                        {
                            sbValues.Append("'").Append(propValue).Append("'");
                        }
                        else
                        {
                            sbValues.Append(propValue);
                        }
                    }
                    else
                    {
                        if (value != null)
                        {
                            if (p.RealType == typeof(string) &&
                                value.GetType() == typeof(string))
                            {
                                value = value.ToString().Replace("'", "\"");
                            }

                            if (p.RealType == typeof(DynamicObjectExt))
                            {
                                value = value.ToString();
                            }
                            else if (p.RealType == typeof(bool))
                            {
                                value = bool.Parse("" + value);
                                if (Features.IsUseIntegerInsteadOfBool(modelDefine))
                                {
                                    value = (bool)value ? 1 : 0;
                                }
                            }
                            else
                            {
                                Type fieldType = CommandUtils.DbType2Type(p.FieldType);
                                if (fieldType != value.GetType())
                                {
                                    value = Convert.ChangeType(value, fieldType);
                                }
                            }
                        }

                        if (j > 0)
                        {
                            sbValues.Append(",");
                        }

                        bool isNeedQuote = (p.FieldType == DbType.String ||
                            p.FieldType == DbType.StringFixedLength ||
                            p.FieldType == DbType.AnsiString ||
                            p.FieldType == DbType.AnsiStringFixedLength ||
                            p.FieldType == DbType.Date ||
                            p.FieldType == DbType.DateTime ||
                            p.FieldType == DbType.DateTime2);
                        if (isNeedQuote)
                        {
                            sbValues.Append("'");
                        }

                        if (p.FieldType == DbType.Date ||
                            p.FieldType == DbType.DateTime ||
                            p.FieldType == DbType.DateTime2)
                        {
                            sbValues.Append(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else
                        {
                            sbValues.Append(value);
                        }

                        if (isNeedQuote)
                        {
                            sbValues.Append("'");
                        }
                    }
                }
            }

            result.SQL = string.Concat("INSERT INTO ", quotes[0], modelDefine.Table, quotes[1], " (", insertFields, ") ", sbValues);
            sbValues.Clear();

            return result;
        }

        public static CommandSQL BuildBatchInsertSQL(Model modelDefine, List<dynamic> batchModelValues)

        {
            ConnectionSetting cs = ConnectionUtils.GetConnectionByModel(modelDefine);
            if ("oracle".Equals(cs.Dialect, StringComparison.OrdinalIgnoreCase))
            {
                return _BuildOracleBatchInsertSQL(modelDefine, batchModelValues);
            }
            else if ("sqlserver".Equals(cs.Dialect, StringComparison.OrdinalIgnoreCase))
            {
                return _BuildSqlserverBatchInsertSQL(modelDefine, batchModelValues);
            }
            else
            {
                string[] quotes = Features.GetObjectQuotes(modelDefine);
                dynamic firstModelValues = batchModelValues[0];

                CommandSQL result = new CommandSQL();

                object value;
                string insertFields = string.Empty;
                List<Property> insertProperties = new List<Property>();
                for (int i = 0; i < modelDefine.PropertyCount; i++)
                {
                    Property p = modelDefine.GetProperty(i);
                    if (p.JoinInsert)
                    {
                        if (modelDefine.TryGetValue(firstModelValues, p, out value))
                        {
                            if (value != null)
                            {
                                if (insertFields.Length > 0)
                                {
                                    insertFields += ",";
                                }
                                insertFields += string.Concat(quotes[0], p.Field, quotes[1]);

                                insertProperties.Add(p);
                            }
                        }
                    }
                }

                StringBuilder sbValues = new StringBuilder(batchModelValues.Count * 100);
                for (int i = 0; i < batchModelValues.Count; i++)
                {
                    dynamic currModelValues = batchModelValues[i];

                    if (sbValues.Length > 0)
                    {
                        sbValues.Append(",");
                    }

                    sbValues.Append("(");
                    for (int j = 0; j < insertProperties.Count; j++)
                    {
                        Property p = insertProperties[j];
                        if (!modelDefine.TryGetValue(currModelValues, p, out value))
                        {
                            value = null;
                        }

                        if (value != null && value is Function)
                        {
                            if (j > 0)
                            {
                                sbValues.Append(",");
                            }

                            sbValues.Append(((Function)value).Convert2SQL(modelDefine));
                        }
                        else if (value != null && value is PropertyValue)
                        {
                            if (j > 0)
                            {
                                sbValues.Append(",");
                            }

                            sbValues.Append(((PropertyValue)value).Convert2SQL(p));
                        }
                        else
                        {
                            if (value != null)
                            {
                                if (p.RealType == typeof(DynamicObjectExt))
                                {
                                    value = value.ToString();
                                }
                                else if (p.RealType == typeof(bool))
                                {
                                    value = bool.Parse("" + value);
                                    if (Features.IsUseIntegerInsteadOfBool(modelDefine))
                                    {
                                        value = (bool)value ? 1 : 0;
                                    }
                                }
                                else
                                {
                                    Type fieldType = CommandUtils.DbType2Type(p.FieldType);
                                    if (fieldType != value.GetType())
                                    {
                                        value = Convert.ChangeType(value, fieldType);
                                    }
                                }
                            }

                            string paramName = CommandUtils.GenParamName(p) + i;
                            DbType dbType = CommandUtils.GetDbParamType(p);
                            DbParameter dp = DbUtils.CreateParam(modelDefine.Path, paramName,
                                value, dbType, ParameterDirection.Input);
                            result.Params.Add(dp);

                            if (j > 0)
                            {
                                sbValues.Append(",");
                            }

                            string paramPlaceholder = Features.GetCommandParamName(modelDefine, paramName);
                            sbValues.Append(paramPlaceholder);
                        }
                    }
                    sbValues.Append(")");
                }

                result.SQL = string.Concat("INSERT INTO ", quotes[0], modelDefine.Table, quotes[1], " (", insertFields, ") VALUES", sbValues);
                sbValues.Clear();

                return result;
            }
        }

        public static CommandSQL BuildInsertSQL(Model m, dynamic modelValues)
        {
            string[] quotes = Features.GetObjectQuotes(m);

            CommandSQL result = new CommandSQL();

            object value;
            string insertFields = string.Empty;
            string insertValues = string.Empty;
            for (int i = 0; i < m.PropertyCount; i++)
            {
                Property p = m.GetProperty(i);
                if (p.JoinInsert)
                {
                    if (m.TryGetValue(modelValues, p, out value))
                    {
                        if (value != null)
                        {
                            if (value is Function)
                            {
                                if (insertFields.Length > 0)
                                {
                                    insertFields += ",";
                                }
                                insertFields += string.Concat(quotes[0], p.Field, quotes[1]);

                                if (insertValues.Length > 0)
                                {
                                    insertValues += ",";
                                }
                                insertValues += ((Function)value).Convert2SQL(m);
                            }
                            else if (value is PropertyValue)
                            {
                                if (insertFields.Length > 0)
                                {
                                    insertFields += ",";
                                }
                                insertFields += string.Concat(quotes[0], p.Field, quotes[1]);

                                if (insertValues.Length > 0)
                                {
                                    insertValues += ",";
                                }
                                insertValues += ((PropertyValue)value).Convert2SQL(p);
                            }
                            else
                            {
                                if (p.RealType == typeof(DynamicObjectExt))
                                {
                                    value = value.ToString();
                                }
                                else if (p.RealType == typeof(Boolean))
                                {
                                    value = bool.Parse("" + value);
                                    if (Features.IsUseIntegerInsteadOfBool(m))
                                    {
                                        value = (bool)value ? 1 : 0;
                                    }
                                }
                                else
                                {
                                    Type fieldType = CommandUtils.DbType2Type(p.FieldType);
                                    if (fieldType != value.GetType())
                                    {
                                        value = Convert.ChangeType(value, fieldType);
                                    }
                                }

                                DbType dbType = CommandUtils.GetDbParamType(p);
                                string paramName = CommandUtils.GenParamName(p);
                                DbParameter dp = DbUtils.CreateParam(m.Path, paramName,
                                    value, dbType, ParameterDirection.Input);
                                result.Params.Add(dp);

                                if (insertFields.Length > 0)
                                {
                                    insertFields += ",";
                                }
                                insertFields += string.Concat(quotes[0], p.Field, quotes[1]);

                                if (insertValues.Length > 0)
                                {
                                    insertValues += ",";
                                }
                                string paramPlaceholder = Features.GetCommandParamName(m, paramName);
                                insertValues += paramPlaceholder;
                            }
                        }
                    }
                }
            }
            result.SQL = string.Concat("INSERT INTO ", quotes[0], m.Table, quotes[1], " (", insertFields, ") VALUES(", insertValues + ")");

            return result;
        }

        public static CommandSQL BuildUpdateSQL(Model m, dynamic modelValues)
        {
            string[] quotes = Features.GetObjectQuotes(m);

            CommandSQL result = new CommandSQL();

            object value;
            StringBuilder sbUpdateContent = new StringBuilder();
            for (int i = 0; i < m.PropertyCount; i++)
            {
                Property p = m.GetProperty(i);
                if (p.JoinUpdate)
                {
                    if (m.TryGetValue(modelValues, p, out value, false))
                    {
                        if (value != null)
                        {
                            if (value is Function)
                            {
                                if (sbUpdateContent.Length > 0)
                                {
                                    sbUpdateContent.Append(",");
                                }
                                sbUpdateContent.Append(quotes[0]).Append(p.Field)
                                    .Append(quotes[1]).Append("=").Append(((Function)value).Convert2SQL(m));
                            }
                            else if (value is PropertyValue)
                            {
                                if (sbUpdateContent.Length > 0)
                                {
                                    sbUpdateContent.Append(",");
                                }
                                sbUpdateContent.Append(quotes[0]).Append(p.Field)
                                    .Append(quotes[1]).Append("=").Append(((PropertyValue)value).Convert2SQL(p));
                            }
                            else
                            {
                                if (p.RealType == typeof(DynamicObjectExt))
                                {
                                    value = value.ToString();
                                }
                                else if (p.RealType == typeof(bool))
                                {
                                    value = bool.Parse("" + value);
                                    if (Features.IsUseIntegerInsteadOfBool(m))
                                    {
                                        value = (bool)value ? 1 : 0;
                                    }
                                }
                                else
                                {
                                    Type fieldType = CommandUtils.DbType2Type(p.FieldType);
                                    if (fieldType != value.GetType())
                                    {
                                        value = Convert.ChangeType(value, fieldType);
                                    }
                                }

                                DbType dbType = CommandUtils.GetDbParamType(p);
                                string paramName = CommandUtils.GenParamName(p);
                                DbParameter dp = DbUtils.CreateParam(m.Path, paramName,
                                    value, dbType, ParameterDirection.Input);
                                result.Params.Add(dp);

                                if (sbUpdateContent.Length > 0)
                                {
                                    sbUpdateContent.Append(",");
                                }
                                string paramPlaceholder = Features.GetCommandParamName(m, paramName);
                                sbUpdateContent.Append(quotes[0]).Append(p.Field)
                                    .Append(quotes[1]).Append("=").Append(paramPlaceholder);
                            }
                        }
                    }
                }
            }
            result.SQL = string.Concat("UPDATE ", quotes[0], m.Table, quotes[1], " SET ", sbUpdateContent);

            CommandSQL where = m.Where.Build(m);
            if (!string.IsNullOrEmpty(where.SQL))
            {
                result.SQL += string.Concat(" WHERE ", where.SQL);
                result.Params.AddRange(where.Params);
                result.FilterProperties = where.FilterProperties;
            }

            return result;
        }

        internal static string BuildJoinTableSQL(Model m, List<string> foreignTables)
        {
            string[] quotes = Features.GetObjectQuotes(m);

            StringBuilder sbJoins = new StringBuilder();

            Hashtable processedName = new Hashtable();
            Hashtable processedModel = new Hashtable();
            foreach (string foreignTableName in foreignTables)
            {
                string key = foreignTableName.Substring(0, foreignTableName.LastIndexOf(".")).Trim().ToLower();
                if (processedName.ContainsKey(key))
                {
                    continue;
                }

                Model currM = m;
                string[] subNames = foreignTableName.Split(".");
                for (int i = 0; i < subNames.Length; i++)
                {
                    Property subProp = currM.GetProperty(subNames[i]);
                    Model subM = ModelUtils.GetModel(subProp.TypeValue);

                    string joinModelName = string.Concat(currM.Name, "_", subM.Name).ToLower();
                    if (processedModel.ContainsKey(joinModelName))
                    {
                        if (i == subNames.Length - 2)
                        {
                            break;
                        }
                        else
                        {
                            currM = subM;
                            continue;
                        }
                    }

                    string joinField = subM.GetPrimaryKey(0).Field;
                    if (!string.IsNullOrWhiteSpace(subProp.JoinProp))
                    {
                        joinField = subM.GetProperty(subProp.JoinProp).Field;
                    }
                    sbJoins.Append(string.Concat(" LEFT JOIN ", quotes[0], subM.Table, quotes[1], " ON ", quotes[0],
                        currM.Table, quotes[1], ".", quotes[0], subProp.Field, quotes[1], "=", quotes[0], subM.Table, quotes[1], ".", quotes[0], joinField, quotes[1]));

                    currM = subM;

                    processedModel.Add(joinModelName, true);

                    if (i == subNames.Length - 2)
                    {
                        break;
                    }
                }

                processedName.Add(key, true);
            }

            return sbJoins.ToString();
        }

        internal static string BuildGroupBySQL(Model m, List<string> foreignTables)
        {
            StringBuilder sbResult = new StringBuilder();

            if (m.GroupByNames.Count > 0)
            {
                sbResult.Append("GROUP BY ");
                foreach (GroupByPart gbp in m.GroupByNames)
                {
                    if (sbResult.Length > 9)
                    {
                        sbResult.Append(",");
                    }
                    sbResult.Append(gbp.Convert2SQL(m));

                    CommandUtils.CheckFunctionForeignProperty(m, gbp.Function, foreignTables);
                }
            }

            return sbResult.ToString();
        }

        internal static CommandSQL BuildQuerySQL(Model m)
        {
            CommandSQL result = new CommandSQL();

            List<SelectFieldPart> queryFields = new List<SelectFieldPart>();
            if (m.ReturnValues.Count > 0)
            {
                queryFields.AddRange(m.ReturnValues);
            }
            else
            {
                for (int i = 0; i < m.PropertyCount; i++)
                {
                    Property p = m.GetProperty(i);
                    queryFields.Add(new SelectFieldPart(Funcs.PROPERTY(p.Name), i + 1));
                }
            }

            List<string> foreignTables = new List<string>();
            StringBuilder sbFields = new StringBuilder();
            foreach (SelectFieldPart sfp in queryFields)
            {
                if (sbFields.Length > 0)
                {
                    sbFields.Append(",");
                }
                sbFields.Append(sfp.Convert2SQL(m));

                CommandUtils.CheckFunctionForeignProperty(m, sfp.Function, foreignTables);
            }

            CommandSQL where = m.Where.Build(m);

            result.FilterProperties = where.FilterProperties;

            foreignTables.AddRange(where.ForeignTables);
            foreignTables.AddRange(m.ForeignSortNames);
            string joinSql = BuildJoinTableSQL(m, foreignTables);

            string[] quotes = Features.GetObjectQuotes(m);
            result.SQL = string.Concat(sbFields, " FROM ", quotes[0], m.Table, quotes[1], joinSql);
            if (!string.IsNullOrEmpty(where.SQL))
            {
                result.SQL += string.Concat(" WHERE ", where.SQL);
                result.Params.AddRange(where.Params);
            }

            if (m.GroupByNames.Count > 0)
            {
                string groupBySQL = BuildGroupBySQL(m, foreignTables);
                if (!string.IsNullOrWhiteSpace(groupBySQL))
                {
                    result.SQL += string.Concat(" ", groupBySQL, " ");
                }
            }

            if (!string.IsNullOrEmpty(m.Sort))
            {
                result.SQL += string.Concat(" ORDER BY ", m.Sort);
            }

            if (m.IsUsePaging)
            {
                result.SQL = Features.GetPagingCommand(m, result.SQL, m.CurrPageSize, m.CurrPageIndex);
                if (result.SQL.Contains("SELECT TOP 9223372036854775807 DISTINCT"))
                {
                    ConnectionSetting cs = ConnectionUtils.GetConnectionByModel(m);
                    if ("sqlserver".Equals(cs.Dialect, StringComparison.OrdinalIgnoreCase))
                    {
                        result.SQL = result.SQL.Replace("TOP 9223372036854775807", "");
                        result.SQL = result.SQL.Replace("SELECT  DISTINCT", "SELECT DISTINCT TOP 9223372036854775807");
                    }
                }
            }
            else
            {
                result.SQL = string.Concat("SELECT ", result.SQL);
            }

            if (m.IsSelectForUpdate)
            {
                result.SQL += " FOR UPDATE";

                if (m.IsNoWait)
                {
                    result.SQL += " NOWAIT";
                }
            }

            return result;
        }

    }
}
