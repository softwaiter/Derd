namespace CodeM.Common.Orm
{
    public interface IPaging
    {

        Model PageSize(int size);

        Model PageIndex(int index);

    }
}
