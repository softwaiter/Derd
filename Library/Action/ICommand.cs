using System.Collections.Generic;

namespace CodeM.Common.Orm
{
    public interface ICommand
    {
        bool CreateTable(bool force=false);

        bool RemoveTable();

        bool TruncateTable();

        bool Save(bool validate = false);

        bool Delete(bool deleteAll = false);

        bool Update(bool updateAll = false);

        List<dynamic> Query();

        long Count();

        bool Exists();

        //In
        //NotIn
    }
}
