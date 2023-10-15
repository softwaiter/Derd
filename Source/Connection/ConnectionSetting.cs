using CodeM.Common.DbHelper;
using System;
using System.Collections.Generic;

namespace CodeM.Common.Orm
{
    public class ConnectionSetting
    {
        /// <summary>
        /// 数据源名称，即connection定义路径
        /// </summary>
        public string DataSource { get; set; }

        public string Dialect { get; set; }

        public string Host { get; set; } = null;

        public int Port { get; set; } = 0;

        public string User { get; set; } = null;

        public string Password { get; set; } = null;

        public string Database { get; set; } = null;

        public bool Encrypt { get; set; } = false;

        public string Charset { get; set; } = null;

        public bool Pooling { get; set; } = false;

        public int MaxPoolSize { get; set; } = 100;

        public int MinPoolSize { get; set; } = 0;

        public int ConnectionTimeout { get; set; } = 0;

        public int CommandTimeout { get; set; } = 0;

        public override string ToString()
        {
            List<string> settings = new List<string>();

            if ("oracle".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
            {
                int connPort = Port > 0 ? Port : 1521;
                string datasourceFormat = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME={2})))";
                settings.Add(string.Format(datasourceFormat, Host, connPort, Database));
            }
            else
            {
                if ("sqlite".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add("Version=3");
                }

                if (!string.IsNullOrWhiteSpace(Host))
                {
                    if ("postgres".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                    {
                        settings.Add(string.Concat("Host=", Host));
                    }
                    else if ("dm".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                    {
                        settings.Add(string.Concat("server=", Host));
                    }
                    else if ("kingbase".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                    {
                        settings.Add(string.Concat("Host=", Host));
                    }
                    else
                    {
                        settings.Add(string.Concat("Data Source=", Host));
                    }
                }

                if ("kingbase".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    Port = Port > 0 ? Port : 54321;
                }

                if (Port > 0)
                {
                    settings.Add(string.Concat("Port=", Port));
                }

                if (!string.IsNullOrWhiteSpace(Database))
                {
                    settings.Add(string.Concat("Database=", Database));
                }

                if (!string.IsNullOrWhiteSpace(Charset))
                {
                    if ("mysql".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                    {
                        settings.Add(string.Concat("Charset=", Charset));
                    }
                    else if ("dm".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                    {
                        settings.Add(string.Concat("encoding=", Charset));
                    }
                    else if ("postgres".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                    {
                        settings.Add(string.Concat("Encoding=", Charset));
                    }
                    else if ("kingbase".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                    {
                        settings.Add(string.Concat("Encoding=", Charset));
                    }
                }

                if ("sqlserver".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    if (Encrypt)
                    {
                        settings.Add("Encrypt=yes");
                    }
                    else
                    {
                        settings.Add("Encrypt=no");
                    }

                    settings.Add("trustServerCertificate=true");
                }
            }

            if (!string.IsNullOrWhiteSpace(User))
            {
                if ("dm".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("user=", User));
                }
                if ("kingbase".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("Username=", User));
                }
                else
                {
                    settings.Add(string.Concat("User Id=", User));
                }
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                settings.Add(string.Concat("Password=", Password));
            }

            if ("dm".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
            {
                settings.Add(string.Concat("conn_pooling=", (Pooling ? "True" : "False")));
            }
            else
            {
                settings.Add(string.Concat("Pooling=", (Pooling ? "True" : "False")));
            }

            if (Pooling)
            {
                if ("dm".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("conn_pool_size=", MaxPoolSize));
                }
                else if ("postgres".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("Maximum Pool Size=", MaxPoolSize));
                    settings.Add(string.Concat("Minimum Pool Size=", MinPoolSize));
                }
                else if ("kingbase".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("Maximum Pool Size=", MaxPoolSize));
                    settings.Add(string.Concat("Minimum Pool Size=", MinPoolSize));
                }
                else
                {
                    settings.Add(string.Concat("Max Pool Size=", MaxPoolSize));
                    settings.Add(string.Concat("Min Pool Size=", MinPoolSize));
                }
            }

            if (ConnectionTimeout > 0)
            {
                if ("dm".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("connect_timeout=", ConnectionTimeout));
                }
                else if ("sqlite".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("busytimeout=", ConnectionTimeout));
                }
                else if ("mysql".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("connectiontimeout=", ConnectionTimeout));
                }
                else if ("sqlserver".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("Connect Timeout=", ConnectionTimeout));
                }
                else if ("oracle".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("CONNECTION TIMEOUT=", ConnectionTimeout));
                }
                else if ("postgres".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("Timeout=", ConnectionTimeout));
                }
                else if ("kingbase".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("Timeout=", ConnectionTimeout));
                }
            }

            if (CommandTimeout > 0)
            {
                DbUtils.DefaultCommandTimeout = CommandTimeout;

                if ("dm".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("command_timeout=", CommandTimeout));
                }
                else if ("sqlite".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("default timeout=", CommandTimeout));
                }
                else if ("mysql".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("defaultcommandtimeout=", CommandTimeout));
                }
                else if ("sqlserver".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("Command Timeout=", CommandTimeout));
                }
                else if ("postgres".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("Command Timeout=", CommandTimeout));
                }
                else if ("kingbase".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    settings.Add(string.Concat("Command Timeout=", CommandTimeout));
                }
            }

            return string.Join(';', settings);
        }

    }
}
