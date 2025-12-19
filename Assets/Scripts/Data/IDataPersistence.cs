public interface IDataPersistence<T> where T : class
{

    /// <summary>
    /// Retrieve game data from <paramref name="data"/> to load into specific variables
    /// </summary>
    /// <param name="data">The file that contains game data</param>
    void LoadData(T data);

    /// <summary>
    /// Add game data that wants to be saved to <paramref name="data"/>
    /// </summary>
    /// <param name="data">The file that contains game data</param>
    void SaveData(ref T data);
}
