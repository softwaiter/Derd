namespace CodeM.Common.Orm
{
    public interface IRuleProcessor
    {
        public void Validate(Property prop, dynamic value);
    }
}
