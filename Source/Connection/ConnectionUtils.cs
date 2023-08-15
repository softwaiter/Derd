using CodeM.Common.DbHelper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CodeM.Common.Orm
{
    internal class ConnectionUtils
    {
        internal static ConcurrentDictionary<string, ConnectionSetting> sConnectionSettings = new ConcurrentDictionary<string, ConnectionSetting>();
        internal static ConcurrentDictionary<int, string> sConnectionSettingIndexes = new ConcurrentDictionary<int, string>();

        internal static ConcurrentDictionary<string, ConnectionSetting> sModelConnections = new ConcurrentDictionary<string, ConnectionSetting>();

        internal static ConcurrentDictionary<string, string> sAllowedDatabases = new ConcurrentDictionary<string, string>();

        static ConnectionUtils()
        {
            //支持的数据库类型
            sAllowedDatabases.TryAdd("sqlite", "System.Data.SQLite.SQLiteFactory, System.Data.SQLite");
            sAllowedDatabases.TryAdd("mysql", "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data");
            sAllowedDatabases.TryAdd("oracle", "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess");
            sAllowedDatabases.TryAdd("sqlserver", "Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient");
            sAllowedDatabases.TryAdd("postgres", "Npgsql.NpgsqlFactory, Npgsql");
            sAllowedDatabases.TryAdd("dm", "Dm.DmClientFactory, DmProvider");
            sAllowedDatabases.TryAdd("kingbase", "Kdbndp.KdbndpFactory, Kdbndp");

            //Postgres个性化设置,Timestamp支持
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        internal static void ClearConnections()
        {
            sConnectionSettings.Clear();
            sConnectionSettingIndexes.Clear();
            sModelConnections.Clear();
        }

        internal static bool AddConnection(string datasource, ConnectionSetting conn)
        {
            if (sConnectionSettings.TryAdd(datasource.ToLower(), conn))
            {
                sConnectionSettingIndexes.AddOrUpdate(sConnectionSettingIndexes.Count, 
                    datasource.ToLower(), (key, value) =>
                {
                    return datasource;
                });
                return true;
            }
            return false;
        }

        internal static ConnectionSetting GetConnection(int index)
        {
            if (index >= 0 && index < sConnectionSettingIndexes.Count)
            {
                return sConnectionSettings[sConnectionSettingIndexes[index]];
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        internal static int ConnectionCount
        {
            get
            {
                return sConnectionSettingIndexes.Count;
            }
        }

        internal static ConnectionSetting GetConnectionByModel(Model model)
        {
            string fullModelName = model.Path;
            if (fullModelName.EndsWith("/"))
            {
                fullModelName += model.Name;
            }
            else
            {
                fullModelName += ("/" + model.Name);
            }
            return GetConnectionByModelName(fullModelName);
        }

        internal static ConnectionSetting GetConnectionByModelName(string modelName)
        {
            string fullModelName = ModelUtils.GetFullModelName(modelName).ToLower();
            if (sModelConnections.ContainsKey(fullModelName))
            {
                ConnectionSetting result;
                if (sModelConnections.TryGetValue(fullModelName, out result))
                {
                    return result;
                }
            }

            string modelPath = fullModelName;
            int pos = modelPath.LastIndexOf("/");
            while (pos >= 0)
            {
                modelPath = modelPath.Substring(0, pos);
                if (pos == 0)
                {
                    modelPath = "/";
                }

                if (sConnectionSettings.ContainsKey(modelPath))
                {
                    ConnectionSetting conns = sConnectionSettings[modelPath];
                    sModelConnections.AddOrUpdate(fullModelName, conns, (key, value) =>
                    {
                        return conns;
                    });
                    return conns;
                }

                if (pos == 0)
                {
                    break;
                }
            }

            throw new Exception("未定义数据源：" + modelName);
        }

        internal static void RegisterAllConnections()
        {
            ConcurrentDictionary<string, bool> dbproviderRegistereds = new ConcurrentDictionary<string, bool>();
            IEnumerator<KeyValuePair<string, ConnectionSetting>> e = sConnectionSettings.GetEnumerator();
            while (e.MoveNext())
            {
                ConnectionSetting conn = e.Current.Value;
                string dialect = conn.Dialect.ToLower();
                if (!dbproviderRegistereds.ContainsKey(dialect))
                {
                    if (sAllowedDatabases.ContainsKey(dialect))
                    {
                        DbUtils.RegisterDbProvider(dialect, sAllowedDatabases[dialect]);
                        dbproviderRegistereds.TryAdd(dialect, true);
                    }
                    else
                    {
                        throw new Exception(string.Concat("不支持的数据库：", conn.Dialect));
                    }
                }

                DbUtils.AddDataSource(conn.DataSource, dialect, conn.ToString());
            }
        }

    }
}
