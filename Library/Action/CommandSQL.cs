using System.Collections.Generic;
using System.Data.Common;

namespace CodeM.Common.Orm
{
    internal class CommandSQL
    {

        public string SQL { get; set; }

        public List<DbParameter> Params { get; set; } = new List<DbParameter>();

    }
}
