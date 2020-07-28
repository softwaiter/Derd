namespace CodeM.Common.Orm
{
    public interface ISort
    {

        Model AscendingSort(string name);

        Model DescendingSort(string name);

    }
}
