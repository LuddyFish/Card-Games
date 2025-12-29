using System.Collections.Generic;
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
            script.Save(script.data, script.GameDataPersistenceObjects);
        if (GUILayout.Button("Destroy Save"))
            script.DestroySaveFile(script.data.handler);

        GUILayout.Space(10);
        if (GUILayout.Button("Save Stats"))
            script.Save(script.stats, script.PlayerStatsPersistenceObjects);
        if (GUILayout.Button("Destroy Stats"))
            script.DestroySaveFile(script.stats.handler);
    }
}

#endif

public class DataPersistenceManager : DataPersistenceBase
{
    public static DataPersistenceManager Instance { get; private set; }

    public DataComponent<GameData> data;
    public DataComponent<PlayerGameStats> stats;

    public List<IDataPersistence<GameData>> GameDataPersistenceObjects => 
        FindAllDataPersistenceObjects<GameData>();
    public List<IDataPersistence<PlayerGameStats>> PlayerStatsPersistenceObjects => 
        FindAllDataPersistenceObjects<PlayerGameStats>();

    protected override void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        base.Awake();
    }

    [Tooltip("State values:\n0: New game\n1: Resume game")]
    private int _enterGameState = 0;
    public int EnterGameState
    {
        get { return _enterGameState; }
        set { _enterGameState = value; }
    }

    public override void Init()
    {
        data.SetHandler();
        Debug.Log("Data file can be found at: " + data.FilePath);
        stats.SetHandler();
        Debug.Log("Stats file can be found at: " + stats.FilePath);

        if (EnterGameState == 1)
            Load(data, GameDataPersistenceObjects);
        else
            New(data);
    }

    public override void LoadAll()
    {
        Load(data, GameDataPersistenceObjects);
        Load(stats, PlayerStatsPersistenceObjects);
    }

    public override void SaveAll()
    {
        Save(data, GameDataPersistenceObjects);
        Save(stats, PlayerStatsPersistenceObjects);
    }

    public override void DestroyAll()
    {
        data.handler?.Delete();
        stats.handler?.Delete();
    }

    public void New<T>(DataComponent<T> component) where T : class, new()
    {
        Debug.Log($"Instatiating new {typeof(T).Name}");
        component.New();
    }
    public void Load<T>(DataComponent<T> component, List<IDataPersistence<T>> dataPersistenceObjects) where T : class, new()
    {
        component.Load();
        foreach (var dataPersistenceObj in dataPersistenceObjects)
            dataPersistenceObj.LoadData(component.data);

        Debug.Log($"Loaded {typeof(T).Name}");
    }

    public void Save<T>(DataComponent<T> component, List<IDataPersistence<T>> dataPersistenceObjects) where T : class, new()
    {
        if (component.data == null)
            component.New();

        foreach (var dataPersistenceObj in dataPersistenceObjects)
            dataPersistenceObj.SaveData(ref component.data);

        component.Save();

        Debug.Log($"Saved {typeof(T).Name}");
    }
    
    /// <summary>
    /// <b>WARNING:</b> Only do this if you're sure you want to remove the existing data
    /// </summary>
    public void DestroySaveFile<T>(FileDataHandler<T> handler) where T : class
    {
        // if handler doesn't point to a file, then determine it's type to be deleted
        handler?.Delete();

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

        Debug.Log($"Deleted {typeof(T).Name} save data");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    protected override void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("MainMenu"))
            SaveAll();
    }

    private List<IDataPersistence<T>> FindAllDataPersistenceObjects<T>() where T : class
    {
        IEnumerable<IDataPersistence<T>> dataPersistenceObjects = 
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence<T>>();

        return new List<IDataPersistence<T>>(dataPersistenceObjects);
    }
}