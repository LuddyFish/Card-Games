using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            script.DestroySaveFile(script.data.handler);

        GUILayout.Space(10);
        if (GUILayout.Button("Save Stats"))
            script.SaveStats();
        if (GUILayout.Button("Destroy Stats"))
            script.DestroySaveFile(script.stats.handler);
    }
}

#endif

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    public DataComponents<GameData> data;
    public DataComponents<PlayerGameStats> stats;

    private List<IDataPersistence> DataPersistenceObjects => FindAllDataPersistenceObjects();

    [System.Serializable]
    public class DataComponents<T> where T : class
    {
        [SerializeField] private string fileName;
        public string FilePath => Path.Combine(Application.persistentDataPath, fileName);

        [HideInInspector] public T data;
        public FileDataHandler<T> handler;

        public void SetHandler() =>
            handler = new FileDataHandler<T>
            (Application.persistentDataPath, fileName);
    }

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
        data.SetHandler();
        Debug.Log("Data file can be found at: " + data.FilePath);
        stats.SetHandler();
        Debug.Log("Stats file can be found at: " + stats.FilePath);

        if (EnterGameState == 1)
            LoadGame();
        else
            NewGame();
    }

    #region Game
    public void NewGame()
    {
        Debug.Log("Instatiating new Game Data");
        data.data = new GameData();
    }

    public void LoadGame()
    {
        // load any saved data from a file
        data.data = data.handler.Load();

        // if no data can be loaded, initilise data
        if (data.data == null)
            NewGame();

        // push loaded data to all other scripts
        foreach (IDataPersistence dataPersistenceObj in DataPersistenceObjects)
            dataPersistenceObj.LoadData(data.data);

        Debug.Log("Loaded game data");
    }

    public void SaveGame()
    {
        // if game data was deleted for some reason
        if (data.data == null)
        {
            NewGame();
        }
        else
        {
            // update existing variables from static classes
            data.data.SaveTableAndDeckData();
        }

        // pass the data to other scripts so they can update
        foreach (IDataPersistence dataPersistenceObj in DataPersistenceObjects)
            dataPersistenceObj.SaveData(ref data.data);

        // save that data to a file
        data.handler.Save(data.data);

        Debug.Log("Saved game data");
    }
    #endregion

    #region Stats
    public void NewStats()
    {
        Debug.Log("Instatiating new Player Game Stats");
        stats.data = new PlayerGameStats();
    }

    public PlayerGameStats GetStats()
    {
        if (stats.data == null)
            NewStats();

        Debug.Log("Retrieving Player Game Stats");
        return stats.data;
    }

    public void SaveStats()
    {
        if (stats.data == null)
            NewStats();

        // pass the data to other scripts so they can update
        foreach (IDataPersistence dataPersistenceObj in DataPersistenceObjects)
           dataPersistenceObj.SaveStats(ref stats.data); 

        // save those stats to a file
        stats.handler.Save(stats.data);

        Debug.Log("Saved player game stats");
    }
    #endregion

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
                    data.SetHandler();
                    data.handler.Delete();
                    break;
                case var t when t == typeof(PlayerGameStats):
                    stats.SetHandler();
                    stats.handler.Delete();
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
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("MainMenu"))
        {
            SaveGame();
            SaveStats();
        }
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = 
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
