using System.Collections.Generic;
using System.Data.Common;

namespace CodeM.Common.Orm
{
    internal class CommandSQL
    {

        public string SQL { get; set; }

        public List<DbParameter> Params { get; } = new List<DbParameter>();

        public List<string> ForeignTables { get; } = new List<string>();

        public Dictionary<string, object> FilterProperties { get; set; } = new Dictionary<string, object>();
    }
}
