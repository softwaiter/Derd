using System;
using System.Collections.Generic;

namespace CodeM.Common.Orm
{
    internal class ConnectionSetting
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

        public override string ToString()
        {
            List<string> settings = new List<string>();

            if ("oracle".Equals(Dialect))
            {
                int connPort = Port > 0 ? Port : 1521;
                string datasourceFormat = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME={2})))";
                settings.Add(string.Format(datasourceFormat, Host, connPort, Database));
            }
            else
            {
                if ("sqlite".Equals(Dialect))
                {
                    settings.Add("Version=3");
                }

                if (!string.IsNullOrWhiteSpace(Host))
                {
                    settings.Add(string.Concat("Data Source=", Host));
                }

                if (Port > 0)
                {
                    settings.Add(string.Concat("Port=", Port));
                }

                if (!string.IsNullOrWhiteSpace(Database))
                {
                    settings.Add(string.Concat("Database=", Database));
                }

                if ("mysql".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(Charset))
                    {
                        settings.Add(string.Concat("Charset=", Charset));
                    }
                }
                else if ("sqlserver".Equals(Dialect, StringComparison.OrdinalIgnoreCase))
                {
                    if (Encrypt)
                    {
                        settings.Add("Encrypt=yes");
                    }
                    else
                    {
                        settings.Add("Encrypt=no");
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(User))
            {
                settings.Add(string.Concat("User Id=", User));
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                settings.Add(string.Concat("Password=", Password));
            }

            //settings.Add(string.Concat("Pooling=", (Pooling ? "True" : "False")));

            //if (Pooling)
            //{
            //    settings.Add(string.Concat("Max Pool Size=", MaxPoolSize));
            //    settings.Add(string.Concat("Min Pool Size=", MinPoolSize));
            //}

            return string.Join(';', settings);
        }

    }
}
