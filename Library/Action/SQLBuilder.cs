using CodeM.Common.DbHelper;
using CodeM.Common.Orm.Dialect;
using CodeM.Common.Tools.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using static CodeM.Common.Orm.Model;

namespace CodeM.Common.Orm
{
    internal class SQLBuilder
    {
        public static CommandSQL BuildInsertSQL(Model m)
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
                    if (m.Values.TryGetValue(p.Name, out value))
                    {
                        if (value != null)
                        {
                            if (p.RealType == typeof(DynamicObjectExt))
                            {
                                value = value.ToString();
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
            result.SQL = string.Concat("INSERT INTO ", quotes[0], m.Table, quotes[1], " (", insertFields, ") VALUES(", insertValues + ")");

            return result;
        }

        public static CommandSQL BuildUpdateSQL(Model m)
        {
            string[] quotes = Features.GetObjectQuotes(m);

            CommandSQL result = new CommandSQL();

            string updateContent = string.Empty;
            for (int i = 0; i < m.PropertyCount; i++)
            {
                Property p = m.GetProperty(i);
                if (p.JoinUpdate)
                {
                    if (m.Values.Has(p.Name))
                    {
                        DbType dbType = CommandUtils.GetDbParamType(p);
                        string paramName = CommandUtils.GenParamName(p);
                        DbParameter dp = DbUtils.CreateParam(m.Path, paramName,
                            m.Values[p.Name], dbType, ParameterDirection.Input);
                        result.Params.Add(dp);

                        if (updateContent.Length > 0)
                        {
                            updateContent += ",";
                        }
                        string paramPlaceholder = Features.GetCommandParamName(m, paramName);
                        updateContent += string.Concat(quotes[0], p.Field, quotes[1], "=", paramPlaceholder);
                    }
                }
            }
            result.SQL = string.Concat("UPDATE ", quotes[0], m.Table, quotes[1], " SET ", updateContent);

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

        private static string GenAggregateTypeField(AggregateType type, string field)
        {
            switch (type)
            {
                case AggregateType.COUNT:
                    return string.Concat("COUNT(", field, ")");
                case AggregateType.DISTINCT:
                    return string.Concat("DISTINCT(", field, ")");
                case AggregateType.SUM:
                    return string.Concat("SUM(", field, ")");
                case AggregateType.MAX:
                    return string.Concat("MAX(", field, ")");
                case AggregateType.MIN:
                    return string.Concat("MIN(", field, ")");
                case AggregateType.AVG:
                    return string.Concat("AVG(", field, ")");
            }
            return field;
        }

        private static string GenFunctionTypeField(Model m, FunctionType type, string field)
        {
            switch (type)
            {
                case FunctionType.DATE:
                    string funcName = Enum.GetName(typeof(FunctionType), type);
                    return Features.GetFunctionCommand(m, funcName, field);
            }
            return field;
        }

        internal static string GenQueryField(Model m, GetValueSetting gvs, string field)
        {
            string qryField = field;
            for (int i = gvs.Operations.Count - 1; i >= 0; i--)
            {
                dynamic _typ = gvs.Operations[i];
                if (_typ is AggregateType)
                {
                    qryField = GenAggregateTypeField(_typ, qryField);
                }
                else if (_typ is FunctionType)
                {
                    qryField = GenFunctionTypeField(m, _typ, qryField);
                }
            }
            return qryField;
        }

        internal static string BuildGroupBySQL(Model m)
        {
            string[] quotes = Features.GetObjectQuotes(m);

            StringBuilder sbResult = new StringBuilder();

            if (m.GroupByNames.Count > 0)
            {
                sbResult.Append("GROUP BY ");
                foreach (GroupBySetting gbs in m.GroupByNames)
                {
                    if (!gbs.Name.Contains("."))    //直接属性
                    {
                        Property p = m.GetProperty(gbs.Name);
                        if (sbResult.Length > 9)
                        {
                            sbResult.Append(",");
                        }

                        string groupField = string.Concat(quotes[0], m.Table, quotes[1], ".", quotes[0], p.Field, quotes[1]);
                        if (gbs.FunctionType != FunctionType.NONE)
                        {
                            string funcName = Enum.GetName(typeof(FunctionType), gbs.FunctionType);
                            string groupFuncField = Features.GetFunctionCommand(m, funcName, groupField);
                            sbResult.Append(groupFuncField);
                        }
                        else
                        {
                            sbResult.Append(groupField);
                        }
                    }
                    else    //Model属性引用
                    {
                        Model currM = m;
                        string[] subNames = gbs.Name.Split(".");
                        for (int i = 0; i < subNames.Length; i++)
                        {
                            Property subProp = currM.GetProperty(subNames[i]);
                            currM = ModelUtils.GetModel(subProp.TypeValue);

                            if (i == subNames.Length - 2)
                            {
                                if (sbResult.Length > 9)
                                {
                                    sbResult.Append(",");
                                }

                                Property lastProp = currM.GetProperty(subNames[i + 1]);
                                string groupField = string.Concat(quotes[0], currM.Table, quotes[1], ".", quotes[0], lastProp.Field, quotes[1]);
                                if (gbs.FunctionType != FunctionType.NONE)
                                {
                                    string funcName = Enum.GetName(typeof(FunctionType), gbs.FunctionType);
                                    string groupFuncField = Features.GetFunctionCommand(m, funcName, groupField);
                                    sbResult.Append(groupFuncField);
                                }
                                else
                                {
                                    sbResult.Append(groupField);
                                }

                                break;
                            }
                        }
                    }
                }
            }

            return sbResult.ToString();
        }

        internal static CommandSQL BuildQuerySQL(Model m)
        {
            string[] quotes = Features.GetObjectQuotes(m);
            string[] aliasQuotes = new string[] { quotes[0], quotes[1] };
            string[] specAliasQuotes = Features.GetFieldAliasQuotes(m);
            if (specAliasQuotes.Length > 0)
            {
                aliasQuotes = specAliasQuotes;
            }

            CommandSQL result = new CommandSQL();

            List<GetValueSetting> queryFields = new List<GetValueSetting>();
            if (m.ReturnValues.Count > 0)
            {
                queryFields.AddRange(m.ReturnValues);
            }
            else
            {
                for (int i = 0; i < m.PropertyCount; i++)
                {
                    queryFields.Add(new GetValueSetting(m.GetProperty(i).Name));
                }
            }

            List<string> foreignTables = new List<string>();
            StringBuilder sbFields = new StringBuilder();
            foreach (GetValueSetting gvs in queryFields)
            {
                if (!gvs.Name.Contains("."))    //直接属性
                {
                    Property p = m.GetProperty(gvs.Name);
                    if (sbFields.Length > 0)
                    {
                        sbFields.Append(",");
                    }
                    sbFields.Append(string.Concat(
                        GenQueryField(m, gvs, string.Concat(quotes[0], m.Table, quotes[1], ".", quotes[0], p.Field, quotes[1])),
                        " AS ", aliasQuotes[0], gvs.FieldName, aliasQuotes[1]));
                }
                else    //Model属性引用
                {
                    foreignTables.Add(gvs.Name);

                    Model currM = m;
                    string[] subNames = gvs.Name.Split(".");
                    for (int i = 0; i < subNames.Length; i++)
                    {
                        Property subProp = currM.GetProperty(subNames[i]);
                        currM = ModelUtils.GetModel(subProp.TypeValue);

                        if (i == subNames.Length - 2)
                        {
                            if (sbFields.Length > 0)
                            {
                                sbFields.Append(",");
                            }

                            Property lastProp = currM.GetProperty(subNames[i + 1]); 
                            sbFields.Append(string.Concat(
                                GenQueryField(currM, gvs, string.Concat(quotes[0], currM.Table, quotes[1], ".", quotes[0], lastProp.Field, quotes[1])),
                                " AS ", aliasQuotes[0], gvs.FieldName, aliasQuotes[1]));

                            break;
                        }
                    }
                }
            }

            CommandSQL where = m.Where.Build(m);

            foreignTables.AddRange(where.ForeignTables);
            foreignTables.AddRange(m.ForeignSortNames);
            string joinSql = BuildJoinTableSQL(m, foreignTables);
            
            result.SQL = string.Concat(sbFields, " FROM ", quotes[0], m.Table, quotes[1], joinSql);
            if (!string.IsNullOrEmpty(where.SQL))
            {
                result.SQL += string.Concat(" WHERE ", where.SQL);
                result.Params.AddRange(where.Params);
            }

            if (m.GroupByNames.Count > 0)
            {
                string groupBySQL = BuildGroupBySQL(m);
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
