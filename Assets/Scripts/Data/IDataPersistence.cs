public interface IDataPersistence
{

    /// <summary>
    /// Retrieve game data from <paramref name="data"/> to load into specific variables
    /// </summary>
    /// <param name="data">The file that contains game data</param>
    void LoadData(GameData data);

    /// <summary>
    /// Add game data that wants to be saved to <paramref name="data"/>
    /// </summary>
    /// <param name="data">The file that contains game data</param>
    void SaveData(ref GameData data);
}
