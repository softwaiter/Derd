using System.Collections.Generic;
using System.Data.Common;

namespace CodeM.Common.Orm
{
    internal class ActionSQL
    {

        public string Command { get; set; }

        public List<DbParameter> Params { get; set; } = new List<DbParameter>();

    }
}
