﻿using System;
using System.Collections;
using System.Data;

namespace CodeM.Common.Orm.SQL.Dialect
{
    internal class Config
    {
        /// <summary>
        /// 配置各数据库对各种特性的支持情况
        /// </summary>
        static Hashtable sFeaturesSupported = Hashtable.Synchronized(new Hashtable()
        {
            { "unsigned", new Hashtable() {
                { "default", false },
                { "mysql", true }
            }},
            { "comment", new Hashtable() {
                { "default", true },
                { "sqlite", false }
            }},
            { "comment_ext_format", new Hashtable() {   //输入table、column、column-description
                { "default", "" },
                { "sqlserver", "execute sp_addextendedproperty N'MS_Description',N'{2}',N'SCHEMA',N'dbo',N'table',N'{0}',N'column',N'{1}'" },
                { "oracle", "comment on column \"{0}\".\"{1}\" is '{2}'" },
                { "postgres", "comment on column {0}.{1} is '{2}'" },
                { "dm", "COMMENT ON COLUMN \"{0}\".\"{1}\" IS '{2}'" },
                { "kingbase", "comment on column \"{0}\".\"{1}\" is '{2}'" }
            }},
            { "create_suffix", new Hashtable() {
                { "default", false },
                { "dm", true }
            }},
            { "create_suffix_ext_format", new Hashtable() { // 输入database
                { "default", "" },
                { "dm", "STORAGE(ON \"{0}\", CLUSTERBTR)" }
            }},
            { "autoincrement", new Hashtable() {
                { "default", true }
            }},
            { "autoincrement_type_replace", new Hashtable() {
                { "default", "" },
                { "postgres", "serial" },
                { "kingbase", "AUTO_INCREMENT" }
            }},
            { "autoincrement_ext_format", new Hashtable() { // 输入table、column、对象标识名称
                { "default", new string[] { } },
                { "oracle", new string[] { "CREATE SEQUENCE SEQ${2} INCREMENT BY 1 START WITH 1 NOMAXVALUE NOCYCLE", "CREATE TRIGGER TRG${2} BEFORE insert ON \"{0}\" FOR EACH ROW begin select SEQ${2}.nextval into:New.\"{1}\" from dual;end;" } }
            }},
            { "autoincrement_gc_ext_format", new Hashtable() { //输入对象标识名称
                { "default", new string[] { } },
                { "oracle", new string[] { "DROP SEQUENCE SEQ${0}" } }
            }},
            { "truncate", new Hashtable() {
                { "default", true },
                { "sqlite", false }
            }},
            { "exists_sql_format", new Hashtable() {    // 输入database、table
                { "sqlite", "SELECT COUNT(*) AS c FROM Sqlite_master WHERE type ='table' AND name ='{1}'" },
                { "mysql", "SELECT COUNT(*) FROM information_schema.TABLES t WHERE t.TABLE_SCHEMA ='{0}' AND t.TABLE_NAME ='{1}'" },
                { "oracle", "SELECT COUNT(*) FROM user_tables t WHERE table_name='{1}'" },
                { "sqlserver", "SELECT COUNT(*) FROM sysObjects WHERE Id=OBJECT_ID(N'{1}') and xtype='U'" },
                { "postgres", "SELECT COUNT(*) FROM pg_class WHERE relname = '{1}'" },
                { "dm", "SELECT COUNT(*) FROM sysobjects WHERE NAME='{1}' AND SCHID IN (SELECT ID FROM sysobjects WHERE NAME=UPPER('{0}'))" },
                { "kingbase", "SELECT COUNT(*) FROM user_tables t WHERE table_name='{1}'" }
            }},
            { "select_forupdate", new Hashtable() {
                { "default", true },
                { "sqlite", false },
                { "sqlserver", false }
            }},
            { "object_quote", new Hashtable() {
                { "default", new string[] { "`", "`" } },
                { "sqlserver", new string[] { "[", "]" } },
                { "oracle", new string[] { "\"", "\"" } },
                { "postgres", new string[] { "\"", "\"" } },
                { "dm", new string[] { "\"", "\"" } },
                { "kingbase", new string[] { "\"", "\"" } }
            }},
            { "field_alias_quote", new Hashtable() {
                { "default", new string[] { } },
                { "oracle", new string[] { "\"", "\"" } },
                { "kingbase", new string[] { "\"", "\"" } }
            }},
            { "command_param_format", new Hashtable() { // 输入paramname
                { "default", "?" },
                { "sqlserver", "@{0}" },
                { "oracle", ":{0}" },
                { "postgres", "@{0}" },
                { "dm", ":{0}" },
                { "kingbase", ":{0}" }
            }},
            { "paging_command_format", new Hashtable() {    // 输入sql、pagesize、pageindex、offset、limit
                { "default", "SELECT {0} LIMIT {3}, {1}" },
                { "sqlserver", "SELECT TOP {1} R.* from (SELECT ROW_NUMBER() OVER(ORDER BY (SELECT 0)) AS RN, * FROM (SELECT TOP 9223372036854775807 {0}) AS Q) AS R WHERE RN > {3}" },
                { "oracle", "SELECT R.* FROM (SELECT ROWNUM RN, Q.* FROM (SELECT {0}) Q) R WHERE R.RN BETWEEN {3} AND {4}" },
                { "postgres", "SELECT {0} LIMIT {1} OFFSET {3}" }
            }},
            { "exec_multi_command", new Hashtable() {
                { "default", true },
                { "oracle", false },
                { "dm", false }
            }},
            { "boolean_is_int", new Hashtable() {
                { "default", false },
                { "sqlserver", true },
                { "postgres", true },
                { "oracle", true }
            }},
            { "string_max_len", new Hashtable() {
                { "default", 8000 },
                { "oracle", 4000 }
            }},
            { "large_text_type", new Hashtable() {
                { "default", "text" },
                { "oracle", "clob" }
            }},
            { "orderby_asc", new Hashtable() {  // 输入排序字段
                { "default", "{0} ASC" },
                { "oracle", "{0} ASC NULLS FIRST" },
                { "kingbase", "{0} ASC NULLS FIRST" }
            }},
            { "orderby_desc", new Hashtable() { // 输入排序字段
                { "default", "{0} DESC" },
                { "oracle", "{0} DESC NULLS LAST" },
                { "kingbase", "{0} DESC NULLS LAST" }
            }}
        });

        /// <summary>
        /// 配置数据库字段自增属性的标签
        /// </summary>
        static Hashtable sFieldAutoIncrementTags = Hashtable.Synchronized(new Hashtable()
        {
            { "tag", new Hashtable() {
                { "default", "AUTOINCREMENT" },
                { "mysql", "AUTO_INCREMENT" },
                { "sqlserver", "IDENTITY" },
                { "dm", "IDENTITY" }
            }}
        });

        /// <summary>
        /// 配置各数据库字段类型的默认长度
        /// </summary>
        static Hashtable sFieldDefaultLengths = Hashtable.Synchronized(new Hashtable()
        {
            { DbType.String, new Hashtable() {
                { "default", 100 }
            }},
            { DbType.Boolean, new Hashtable() {
                { "default", 0 }
            }},
            { DbType.SByte, new Hashtable() {
                { "default", 0 },
                { "mysql", 4 }
            }},
            { DbType.Byte, new Hashtable() {
                { "default", 0 },
                { "mysql", 4 }
            }},
            { DbType.Int16, new Hashtable() {
                { "default", 0 },
                { "mysql", 6 }
            }},
            { DbType.UInt16, new Hashtable() {
                { "default", 0 },
                { "mysql", 6 }
            }},
            { DbType.Int32, new Hashtable() {
                { "default", 0 },
                { "mysql", 11 }
            }},
            { DbType.UInt32, new Hashtable() {
                { "default", 0 },
                { "mysql", 11 }
            }},
            { DbType.Int64, new Hashtable() {
                { "default", 0 },
                { "mysql", 20 }
            }},
            { DbType.UInt64, new Hashtable() {
                { "default", 0 },
                { "mysql", 20 }
            }},
            { DbType.Decimal, new Hashtable() {
                { "default", 0 },
                { "mysql", 10 }
            }},
            { DbType.Double, new Hashtable() {
                { "default", 0 }
            }},
            { DbType.Single, new Hashtable() {
                { "default", 0 }
            }},
            { DbType.DateTime, new Hashtable() {
                { "default", 0 }
            }},
            { DbType.Date, new Hashtable() {
                { "default", 0 }
            }},
            { DbType.Time, new Hashtable() {
                { "default", 0 }
            }},
            { DbType.Object, new Hashtable() {
                { "default", 0 }
            }}
        });

        /// <summary>
        /// 配置各数据库物理字段类型的对应字符串表示
        /// </summary>
        static Hashtable sFieldTypes = Hashtable.Synchronized(new Hashtable()
        {
            { DbType.String, new Hashtable() {
                { "default", "varchar" }
            }},
            { DbType.SByte, new Hashtable() {
                { "default", "integer" },
                { "mysql", "tinyint" },
                { "sqlserver", "tinyint" },
                { "postgres", "smallint" },
                { "dm", "tinyint" }
            }},
            { DbType.Byte, new Hashtable() {
                { "default", "integer" },
                { "mysql", "tinyint" },
                { "sqlserver", "tinyint" },
                { "postgres", "smallint" },
                { "dm", "tinyint" }
            }},
            { DbType.Int16, new Hashtable() {
                { "default", "integer" },
                { "mysql", "smallint" },
                { "sqlserver", "smallint" },
                { "postgres", "smallint" },
                { "dm", "smallint" }
            }},
            { DbType.UInt16, new Hashtable() {
                { "default", "integer" }
            }},
            { DbType.Int32, new Hashtable() {
                { "default", "integer" }
            }},
            { DbType.UInt32, new Hashtable() {
                { "default", "bigint" }
            }},
            { DbType.Int64, new Hashtable() {
                { "default", "bigint" },
                { "oracle", "long" }
            }},
            { DbType.UInt64, new Hashtable() {
                { "default", "bigint" },
                { "oracle", "long" }
            }},
            { DbType.Single, new Hashtable() {
                { "default", "float" },
                { "postgres", "float4" },
            }},
            { DbType.Decimal, new Hashtable() {
                { "default", "decimal" },
                { "sqlite", "numeric" },
                { "postgres", "numeric" }
            }},
            { DbType.Double, new Hashtable() {
                { "default", "double" },
                { "sqlite", "real" },
                { "sqlserver", "real" },
                { "oracle", "float" },
                { "postgres", "float8" },
            }},
            { DbType.Boolean, new Hashtable() {
                { "default", "boolean" },
                { "sqlserver", "bit" },
                { "postgres", "smallint" },
                { "oracle", "number" },
                { "dm", "bit" }
            }},
            { DbType.DateTime, new Hashtable() {
                { "default", "datetime" },
                { "oracle", "timestamp" },
                { "postgres", "timestamp" }
            }},
            { DbType.Date, new Hashtable() {
                { "default", "date" }
            }},
            { DbType.Time, new Hashtable() {
                { "default", "time" },
                { "oracle", "timestamp" }
            }},
            { DbType.Object, new Hashtable() {
                { "default", "blob" },
                { "sqlserver", "varbinary" },
                { "postgres", "bytea" }
            }}
        });

        static Hashtable sFunctions = Hashtable.Synchronized(new Hashtable()
        {
            { "PROPERTY", new Hashtable() {
                { "default", "{0}" }
            }},
            { "DATETIME", new Hashtable() {
                { "default", "TO_CHAR({0}, 'YYYY-MM-DD HH24:mi:ss')" },
                { "sqlite", "DATETIME({0})" },
                { "mysql", "DATE_FORMAT({0}, '%Y-%m-%d %H:%i:%s')" },
                { "sqlserver", "CONVERT(VARCHAR(19), {0}, 120)" }
            }},
            { "DATE", new Hashtable() {
                { "default", "TO_CHAR({0}, 'YYYY-MM-DD')" },
                { "sqlite", "DATE({0})" },
                { "mysql", "DATE_FORMAT({0}, '%Y-%m-%d')" },
                { "sqlserver", "CONVERT(VARCHAR(10), {0}, 120)" }
            }},
            { "TIME", new Hashtable() {
                { "default", "TO_CHAR({0}, 'HH24:mi:ss')" },
                { "sqlite", "TIME({0})" },
                { "mysql", "TIME_FORMAT({0}, '%H:%i:%s')" },
                { "sqlserver", "CONVERT(VARCHAR(8), {0}, 8)" }
            }},
            { "COUNT", new Hashtable() {
                { "default", "COUNT({0})" }
            }},
            { "DISTINCT", new Hashtable() {
                { "default", "DISTINCT({0})" }
            }},
            { "SUM", new Hashtable() {
                { "default", "SUM({0})" }
            }},
            { "MAX", new Hashtable() {
                { "default", "MAX({0})" }
            }},
            { "MIN", new Hashtable() {
                { "default", "MIN({0})" }
            }},
            { "AVG", new Hashtable() {
                { "default", "AVG({0})" }
            }},
            { "SUBSTR", new Hashtable() {
                { "default", "SUBSTR({0}, {1}, {2})" },
                { "sqlserver", "SUBSTRING({0}, {1}, {2})" }
            }},
            { "LENGTH", new Hashtable() {
                { "default", "LENGTH({0})" },
                { "sqlserver", "LEN({0})" }
            }},
            { "UPPER", new Hashtable() {
                { "default", "UPPER({0})" }
            }},
            { "LOWER", new Hashtable() {
                { "default", "LOWER({0})" }
            }},
            { "LTRIM", new Hashtable() {
                { "default", "LTRIM({0})" }
            }},
            { "RTRIM", new Hashtable() {
                { "default", "RTRIM({0})" }
            }},
            { "TRIM", new Hashtable() {
                { "default", "TRIM({0})" },
                { "sqlserver", "LTRIM(RTRIM({0}))" }
            }},
            { "ABS", new Hashtable() {
                { "default", "ABS({0})" }
            }},
            { "ROUND", new Hashtable() {
                { "default", "ROUND({0}, {1})" }
            }}
        });

        static Hashtable sFunctionReturnTypes = Hashtable.Synchronized(new Hashtable()
        {
            { "DATETIME", new Hashtable() {
                { "default", "String" }
            }},
            { "DATE", new Hashtable() {
                { "default", "String" }
            }},
            { "TIME", new Hashtable() {
                { "default", "String" }
            }},
            { "COUNT", new Hashtable() {
                { "default", "Int64" }
            }},
            { "DISTINCT", new Hashtable() {
                { "default", "" }
            }},
            { "SUM", new Hashtable() {
                { "default", "" }
            }},
            { "MAX", new Hashtable() {
                { "default", "" }
            }},
            { "MIN", new Hashtable() {
                { "default", "" }
            }},
            { "AVG", new Hashtable() {
                { "default", "Decimal" }
            }},
            { "SUBSTR", new Hashtable() {
                { "default", "String" }
            }},
            { "LENGTH", new Hashtable() {
                { "default", "Int32" }
            }},
            { "UPPER", new Hashtable() {
                { "default", "String" }
            }},
            { "LOWER", new Hashtable() {
                { "default", "String" }
            }},
            { "LTRIM", new Hashtable() {
                { "default", "String" }
            }},
            { "RTRIM", new Hashtable() {
                { "default", "String" }
            }},
            { "TRIM", new Hashtable() {
                { "default", "String" }
            }},
            { "ABS", new Hashtable() {
                { "default", "" }
            }},
            { "ROUND", new Hashtable() {
                { "default", "" }
            }}
        });

        /// <summary>
        /// 配置项汇总访问入口
        /// </summary>
        static Hashtable sConfigItems = Hashtable.Synchronized(new Hashtable()
        {
            { "feature", sFeaturesSupported },
            { "fieldtype", sFieldTypes },
            { "fieldlength", sFieldDefaultLengths },
            { "autoincrtag", sFieldAutoIncrementTags },
            { "function", sFunctions },
            { "functiontype", sFunctionReturnTypes }
        });

        internal static object GetConfigValue(string config, Model model, object key)
        {
            if (sConfigItems.ContainsKey(config))
            {
                object result;

                Hashtable items = sConfigItems[config] as Hashtable;
                if (items != null && items.ContainsKey(key))
                {
                    Hashtable data = items[key] as Hashtable;
                    ConnectionSetting cs = ConnectionUtils.GetConnectionByModel(model);
                    if (cs != null && data.ContainsKey(cs.Dialect))
                    {
                        result = data[cs.Dialect];
                    }
                    else
                    {
                        result = data["default"];
                    }

                    return result;
                }
                throw new NotSupportedException(string.Concat(config, "配置中未找到指定项 ", key, "。"));
            }
            throw new NotSupportedException(string.Concat(config, "配置项不存在。"));
        }

        internal static T GetConfigValue<T>(string config, Model model, object key)
        {
            object result = GetConfigValue(config, model, key);
            if (result != null)
            {
                return (T)result;
            }
            return default(T);
        }

    }
}
