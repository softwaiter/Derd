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

        public bool Pooling { get; set; } = false;

        public int MaxPoolSize { get; set; } = 100;

        public int MinPoolSize { get; set; } = 0;

        public override string ToString()
        {
            List<string> settings = new List<string>();

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

            if (!string.IsNullOrWhiteSpace(User))
            {
                settings.Add(string.Concat("User Id=", User));
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                settings.Add(string.Concat("Password=", Password));
            }

            if (!string.IsNullOrWhiteSpace(Database))
            {
                settings.Add(string.Concat("Database=", Database));
            }

            settings.Add(string.Concat("Pooling=", (Pooling ? "True" : "False")));

            if (Pooling)
            {
                settings.Add(string.Concat("Max Pool Size=", MaxPoolSize));
                settings.Add(string.Concat("Min Pool Size=", MinPoolSize));
            }

            return string.Join(';', settings);
        }

    }
}
