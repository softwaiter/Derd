using System.Collections.Generic;
using System.Text;

namespace CodeM.Common.Orm
{
    public class ConnectionSetting
    {
        /// <summary>
        /// 数据源名称，即connection定义路径
        /// </summary>
        public string DataSource { get; set; }

        public string Dialect { get; set; }

        public string Host { get; set; }

        public int Port { get; set; } = 0;

        public string User { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }

        public bool Pooling { get; set; } = false;

        public int MaxPoolSize { get; set; } = 100;

        public int MinPoolSize { get; set; } = 0;

        public override string ToString()
        {
            List<string> settings = new List<string>();

            if (!string.IsNullOrWhiteSpace(Database))
            {
                settings.Add(string.Concat("Version=3;Data Source=", Database));
            }

            return string.Join(';', settings);
        }

    }
}
