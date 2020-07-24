using System.Collections.Generic;

namespace CodeM.Common.Orm
{
    public interface ICommand
    {

        bool Save();

        bool Delete(bool deleteAll = false);

        bool Update(bool updateAll = false);

        List<ModelObject> Query();

        long Count();

        bool Exists();

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

        //public ICommand OrderBy(string properyName, bool desc);
    }
}
