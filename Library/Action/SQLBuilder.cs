using CodeM.Common.DbHelper;
using System;
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

            object value;
            string updateContent = string.Empty;
            for (int i = 0; i < m.PropertyCount; i++)
            {
                Property p = m.GetProperty(i);
                if (p.JoinUpdate)
                {
                    if (m.Values.TryGetValue(p.Name, out value))
                    {
                        DbParameter dp = DbUtils.CreateParam(m.Path, Guid.NewGuid().ToString("N"),
                            value, p.FieldType, ParameterDirection.Input);
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

        internal static CommandSQL BuildQuerySQL(Model m)
        {
            //TODO Join

            CommandSQL result = new CommandSQL();

            StringBuilder sb = new StringBuilder();
            if (m.ReturnValues.Count > 0)
            {
                foreach (string name in m.ReturnValues)
                {
                    Property p = m.GetProperty(name);
                    if (sb.Length > 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append(string.Concat(p.Field, " AS ", name));
                }
            }
            else
            {
                sb.Append("*");
            }
            result.SQL = string.Concat("SELECT ", sb, " FROM ", m.Table);

            CommandSQL where = m.Where.Build(m);
            if (!string.IsNullOrEmpty(where.SQL))
            {
                result.SQL += string.Concat(" WHERE ", where.SQL);
                result.Params.AddRange(where.Params);
            }

            if (m.IsUsePaging)
            {
                result.SQL += string.Concat(" LIMIT ", (m.CurrPageIndex - 1) * m.CurrPageSize, ",", m.CurrPageSize);
            }

            return result;
        }

    }
}
