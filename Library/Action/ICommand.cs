using System.Collections.Generic;

namespace CodeM.Common.Orm
{
    public interface ICommand
    {
        bool CreateTable(bool force=false);

        bool RemoveTable();

        bool TruncateTable();

        bool Save(bool validate = false);

        bool Save(int? transCode, bool validate = false);

        bool Delete(bool deleteAll = false);

        bool Delete(int? transCode, bool deleteAll = false);

        bool Update(bool updateAll = false);

        bool Update(int? transCode, bool updateAll = false);

        List<dynamic> Query(int? transCode);

        long Count(int? transCode);

        bool Exists(int? transCode);

        //In
        //NotIn
    }
}
