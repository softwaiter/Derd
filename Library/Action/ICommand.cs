using System.Collections.Generic;

namespace CodeM.Common.Orm
{
    public interface ICommand
    {

        bool Save();

        bool Delete(bool deleteAll = false);

        bool Update(bool updateAll = false);

        long Count();

        //Like
        //NotLike
        //In
        //NotIn
        //IsNull
        //NotIsNull


        //public ICommand NotEquals();

        //public ICommand GreaterThan();

        //public ICommand GreaterThanOrEquals();

        //Lt
        //Lte
        //Bettwen

        //public ICommand PageSize(int pagesize);

        //public ICommand PageIndex(int pageindex);

        //public ICommand Top(int num);

        //public ICommand OrderBy(string properyName, bool desc);

        //public bool Query();

        //public bool Exists();
    }
}
