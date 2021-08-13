using System;
using System.Collections;
using System.Data;

namespace CodeM.Common.Orm.Dialect
{
    internal class Config
    {
        /// <summary>
        /// 配置各数据库对各种特性的支持情况
        /// </summary>
        static Hashtable sFeaturesSupported = Hashtable.Synchronized(new Hashtable()
        {
            { "unsigned", new Hashtable() {
                { "default", true },
                { "sqlite", false }
            }},
            { "comment", new Hashtable() {
                { "default", true },
                { "sqlite", false }
            }},
            { "autoincrement", new Hashtable() {
                { "default", true },
                { "oracle", false }
            }},
            { "truncate", new Hashtable() {
                { "default", true },
                { "sqlite", false }
            }},
            { "exists_sql", new Hashtable() {
                { "sqlite", "select count(*) as c from Sqlite_master where type ='table' and name ='{0}'" },
                { "mysql", "select count(*)  from information_schema.TABLES t where t.TABLE_SCHEMA ='{1}' and t.TABLE_NAME ='{0}'" },
                { "oracle", "select count(*) from user_tables t where table_name=upper('{0}')" },
                { "sqlserver", "select count(*) from sysobjects where id = object_id('{1}.Owner.{0}')" }
            }},
            { "select_forupdate", new Hashtable() {
                { "default", true },
                { "sqlite", false }
            }}
        });

        /// <summary>
        /// 配置数据库字段自增属性的标签
        /// </summary>
        static Hashtable sFieldAutoIncrementTags = Hashtable.Synchronized(new Hashtable()
        {
            { "tag", new Hashtable() {
                { "default", "AUTOINCREMENT" },
                { "mysql", "AUTO_INCREMENT" }
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
                { "mysql", "tinyint" }
            }},
            { DbType.Byte, new Hashtable() {
                { "default", "integer" },
                { "mysql", "tinyint" }
            }},
            { DbType.Int16, new Hashtable() {
                { "default", "integer" },
                { "mysql", "smallint" }
            }},
            { DbType.UInt16, new Hashtable() {
                { "default", "integer" },
                { "mysql", "smallint" }
            }},
            { DbType.Int32, new Hashtable() {
                { "default", "integer" }
            }},
            { DbType.UInt32, new Hashtable() {
                { "default", "integer" }
            }},
            { DbType.Int64, new Hashtable() {
                { "default", "integer" },
                { "mysql", "bigint" }
            }},
            { DbType.UInt64, new Hashtable() {
                { "default", "integer" },
                { "mysql", "bigint" }
            }},
            { DbType.Single, new Hashtable() {
                { "default", "float" }
            }},
            { DbType.Decimal, new Hashtable() {
                { "default", "decimal" },
                { "sqlite", "numeric" }
            }},
            { DbType.Double, new Hashtable() {
                { "default", "double" },
                { "sqlite", "real" }
            }},
            { DbType.Boolean, new Hashtable() {
                { "default", "boolean" }
            }},
            { DbType.DateTime, new Hashtable() {
                { "default", "datetime" }
            }},
            { DbType.Date, new Hashtable() {
                { "default", "date" }
            }},
            { DbType.Time, new Hashtable() {
                { "default", "time" }
            }},
            { DbType.Object, new Hashtable() {
                { "default", "blob" }
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
            { "autoincrtag", sFieldAutoIncrementTags }
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
