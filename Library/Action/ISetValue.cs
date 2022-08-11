namespace CodeM.Common.Orm
{
    public interface ISetValue
    {
        Model SetValue(string name, object value, bool validate = false);

        Model SetValues(dynamic obj, bool validate = false);

    }
}
