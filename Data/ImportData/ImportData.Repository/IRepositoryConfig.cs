namespace ImportData.Repository
{
    public interface IRepositoryConfig
    {
        IRepositoryConfig TryAddCol(string name, DataType Type);
        IRepositoryConfig AddCol(string name, DataType Type);
        IRepositoryConfig SetTableName(string name);
    }
}
