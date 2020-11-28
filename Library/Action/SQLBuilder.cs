using CodeM.Common.DbHelper;
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

        public static CommandSQL BuildInsertSQL(Model m)
        {
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
                        DbParameter dp = DbUtils.CreateParam(m.Path, Guid.NewGuid().ToString("N"),
                            value, p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);

                        if (insertFields.Length > 0)
                        {
                            insertFields += ",";
                        }
                        insertFields += p.Field;

                        if (insertValues.Length > 0)
                        {
                            insertValues += ",";
                        }
                        insertValues += "?";
                    }
                }
            }
            result.SQL = string.Concat("INSERT INTO ", m.Table, " (", insertFields, ") VALUES(", insertValues + ")");

            return result;
        }

        public static CommandSQL BuildUpdateSQL(Model m)
        {
            CommandSQL result = new CommandSQL();

            string updateContent = string.Empty;
            for (int i = 0; i < m.PropertyCount; i++)
            {
                Property p = m.GetProperty(i);
                if (p.JoinUpdate)
                {
                    if (m.Values.Has(p.Name))
                    {
                        DbParameter dp = DbUtils.CreateParam(m.Path, Guid.NewGuid().ToString("N"),
                            m.Values[p.Name], p.FieldType, ParameterDirection.Input);
                        result.Params.Add(dp);

                        if (updateContent.Length > 0)
                        {
                            updateContent += ",";
                        }
                        updateContent += string.Concat(p.Field, "=?");
                    }
                }
            }
            result.SQL = string.Concat("UPDATE ", m.Table, " SET ", updateContent);

            CommandSQL where = m.Where.Build(m);
            if (!string.IsNullOrEmpty(where.SQL))
            {
                result.SQL += string.Concat(" WHERE ", where.SQL);
                result.Params.AddRange(where.Params);
            }

            return result;
        }

        internal static string BuildJoinTableSQL(Model m, List<string> foreignTables)
        {
            StringBuilder sbJoins = new StringBuilder();

            Hashtable processedName = new Hashtable();
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
                    sbJoins.Append(string.Concat(" LEFT JOIN ", subM.Table, " ON ",
                        currM.Table, ".", subProp.Field, "=", subM.Table, ".", subM.GetPrimaryKey(0).Field));
                    currM = subM;

                    if (i == subNames.Length - 2)
                    {
                        break;
                    }
                }

                processedName.Add(key, true);
            }

            return sbJoins.ToString();
        }

        internal static CommandSQL BuildQuerySQL(Model m)
        {
            CommandSQL result = new CommandSQL();

            List<string> queryFields = new List<string>();
            if (m.ReturnValues.Count > 0)
            {
                queryFields.AddRange(m.ReturnValues);
            }
            else
            {
                for (int i = 0; i < m.PropertyCount; i++)
                {
                    queryFields.Add(m.GetProperty(i).Name);
                }
            }

            List<string> foreignTables = new List<string>();
            StringBuilder sbFields = new StringBuilder();
            foreach (string name in queryFields)
            {
                if (!name.Contains("."))    //直接属性
                {
                    Property p = m.GetProperty(name);
                    if (sbFields.Length > 0)
                    {
                        sbFields.Append(",");
                    }
                    sbFields.Append(string.Concat(m.Table, ".", p.Field, " AS ", name));
                }
                else    //Model属性引用
                {
                    foreignTables.Add(name);

                    Model currM = m;
                    string[] subNames = name.Split(".");
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
                            string fieldName = name.Replace(".", "_");
                            sbFields.Append(string.Concat(currM.Table, ".", lastProp.Field, " AS ", fieldName));

                            break;
                        }
                    }
                }
            }

            CommandSQL where = m.Where.Build(m);

            foreignTables.AddRange(where.ForeignTables);
            foreignTables.AddRange(m.ForeignSortNames);
            string joinSql = BuildJoinTableSQL(m, foreignTables);
            
            result.SQL = string.Concat("SELECT ", sbFields, " FROM ", m.Table, joinSql);
            if (!string.IsNullOrEmpty(where.SQL))
            {
                result.SQL += string.Concat(" WHERE ", where.SQL);
                result.Params.AddRange(where.Params);
            }

            if (!string.IsNullOrEmpty(m.Sort))
            {
                result.SQL += string.Concat(" ORDER BY ", m.Sort);
            }

            if (m.IsUsePaging)
            {
                result.SQL += string.Concat(" LIMIT ", (m.CurrPageIndex - 1) * m.CurrPageSize, ",", m.CurrPageSize);
            }

            return result;
        }

    }
}
