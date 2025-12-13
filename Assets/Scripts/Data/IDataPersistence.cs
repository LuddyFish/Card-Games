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

    /// <summary>
    /// Add game data that the player wants to save to their <paramref name="stats"/>.
    /// </summary>
    /// <param name="stats">The file that contains the Player's game stats</param>
    void SaveStats(ref PlayerGameStats stats);
}
