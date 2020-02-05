using System.Text;

namespace CodeM.Common.Orm
{
    public class ConnectionSetting
    {
        public string ModelPath { get; set; }

        public string Dialect { get; set; }

        public string Host { get; set; }

        public int Port { get; set; } = 0;

        public string User { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }

        public bool Pooling { get; set; } = true;

        public int MaxPoolSize { get; set; } = 100;

        public int MinPoolSize { get; set; } = 0;

        public override string ToString()
        {
            StringBuilder sbResult = new StringBuilder(200);

            return sbResult.ToString();
        }

    }
}
