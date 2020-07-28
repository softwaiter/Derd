using System.Collections.Generic;

namespace CodeM.Common.Orm
{
    public interface ICommand
    {
        bool CreateTable(bool force=false);

        bool RemoveTable();

        bool TruncateTable();

        bool Save();

        bool Delete(bool deleteAll = false);

        bool Update(bool updateAll = false);

        List<dynamic> Query();

        long Count();

        bool Exists();

        //Like
        //NotLike
        //In
        //NotIn
        //IsNull
        //IsNotNull


        //public ICommand GreaterThan();

        //public ICommand GreaterThanOrEquals();

        //Lt
        //Lte
        //Bettwen

        //public ICommand OrderBy(string properyName, bool desc);
    }
}
