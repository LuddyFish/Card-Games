public interface IDataComponent
{
    System.Type DataType { get; }

    void New();
    void Load();
    void Save();
}