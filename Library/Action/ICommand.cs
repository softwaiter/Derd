using System.Collections.Generic;

namespace CodeM.Common.Orm
{
    public interface ICommand
    {

        bool Save();

        //public bool Delete();

        //public bool Update();

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

        //public long Count();

        //public bool Exists();
    }
}
