using CodeM.Common.Orm.Serialize;

namespace CodeM.Common.Orm
{
    public interface ISetValue
    {
        Model SetValue(string name, object value);

        Model SetValues(ModelObject obj);

    }
}
