using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(DataPersistenceManager))]
public class DataScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);

        DataPersistenceManager script = (DataPersistenceManager)target;
        if (GUILayout.Button("Save Game"))
            script.SaveGame();
        if (GUILayout.Button("Destroy Save"))
            script.DestroySaveFile(script.dataHandler);

        GUILayout.Space(10);
        if (GUILayout.Button("Save Stats"))
            script.SaveStats();
        if (GUILayout.Button("Destroy Stats"))
            script.DestroySaveFile(script.statsHandler);
    }
}

#endif

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    [SerializeField] private string dataFileName;
    [SerializeField] private string statsFileName;
    public string DataPath => Path.Combine(Application.persistentDataPath, dataFileName);
    public string StatsPath => Path.Combine(Application.persistentDataPath, statsFileName);

    private GameData gameData;
    private PlayerGameStats playerStats;
    private List<IDataPersistence> DataPersistenceObjects => FindAllDataPersistenceObjects();
    public FileDataHandler<GameData> dataHandler { get; private set; }
    public FileDataHandler<PlayerGameStats> statsHandler { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }

    [Tooltip("State values:\n0: New game\n1: Resume game")]
    private int _enterGameState = 0;
    public int EnterGameState
    {
        get { return _enterGameState; }
        set { _enterGameState = value; }
    }

    public void Init()
    {
        SetDataHandler();
        Debug.Log("Data file can be found at: " + DataPath);
        SetStatsHandler();
        Debug.Log("Stats file can be found at: " + StatsPath);

        if (EnterGameState == 1)
            LoadGame();
        else
            NewGame();
    }

    private void SetDataHandler() => 
        dataHandler = new FileDataHandler<GameData>
        (Application.persistentDataPath, dataFileName);
    private void SetStatsHandler() => 
        statsHandler = new FileDataHandler<PlayerGameStats>
        (Application.persistentDataPath, statsFileName);

    public void NewGame()
    {
        Debug.Log("Instatiating new Game Data");
        gameData = new GameData(Table.Players, Deck.Cards);
    }

    public void LoadGame()
    {
        // load any saved data from a file
        gameData = dataHandler.Load();

        // if no data can be loaded, initilise data
        if (gameData == null)
            NewGame();

        // push loaded data to all other scripts
        foreach (IDataPersistence dataPersistenceObj in DataPersistenceObjects)
            dataPersistenceObj.LoadData(gameData);

        Debug.Log("Loaded game data");
    }

    public void SaveGame()
    {
        // if game data was deleted for some reason
        if (gameData == null)
        {
            NewGame();
        }
        else
        {
            // update existing variables from static classes
            gameData.SaveTableAndDeckData(Table.Players, Deck.Cards);
        }

        // pass the data to other scripts so they can update
        foreach (IDataPersistence dataPersistenceObj in DataPersistenceObjects)
            dataPersistenceObj.SaveData(ref gameData);

        // save that data to a file
        dataHandler.Save(gameData);

        Debug.Log("Saved game data");
    }

    public void NewStats()
    {
        Debug.Log("Instatiating new Player Game Stats");
        playerStats = new PlayerGameStats();
    }

    public PlayerGameStats GetStats()
    {
        if (playerStats == null)
            NewStats();

        Debug.Log("Retrieving Player Game Stats");
        return playerStats;
    }

    public void SaveStats()
    {
        if (playerStats == null)
            NewStats();

        // pass the data to other scripts so they can update
        foreach (IDataPersistence dataPersistenceObj in DataPersistenceObjects)
           dataPersistenceObj.SaveStats(ref playerStats); 

        // save those stats to a file
        statsHandler.Save(playerStats);

        Debug.Log("Saved player game stats");
    }

    /// <summary>
    /// <b>WARNING:</b> Only do this if you're sure you want to remove the existing data
    /// </summary>
    public void DestroySaveFile<T>(FileDataHandler<T> handler) where T : class
    {
        // if handler doesn't point to a file, then determine it's type to be deleted
        if (handler == null)
        {
            Debug.Log($"Could not find {typeof(T).Name} save file");
            switch (typeof(T))
            {
                case var t when t == typeof(GameData):
                    SetDataHandler();
                    dataHandler.Delete();
                    break;
                case var t when t == typeof(PlayerGameStats):
                    SetStatsHandler();
                    statsHandler.Delete();
                    break;
                default: 
                    Debug.LogError($"Class {typeof(T).Name} is not supported"); 
                    return;
            }
        }
        else
        {
            handler.Delete();
        }

        Debug.Log($"Deleted {typeof(T).Name} save data");
    }

    private void OnApplicationQuit()
    {
        SaveGame();
        SaveStats();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = 
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
