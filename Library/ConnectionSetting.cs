namespace CodeM.Common.Orm
{
    public class ConnectionSetting
    {
        
        public string Host { get; set; }

        public int Port { get; set; } = 3306;

        public string User { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }

    }
}
