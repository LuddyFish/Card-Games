using System;
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
            script.DestroySaveFile();
    }
}

#endif

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    [SerializeField] private string fileName;
    [HideInInspector] public string FullPath => Path.Combine(Application.persistentDataPath, fileName);

    private GameData gameData;
    private readonly List<IDataPersistence> registeredObjects = new();
    private FileDataHandler<GameData> dataHandler;

    [Tooltip("FALSE = New game | TRUE = Resume game")]
    private bool _resumeGame = false;
    public bool ResumeGame
    {
        get { return _resumeGame; }
        set { _resumeGame = value; }
    }

    protected void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public void Init()
    {
        SetDataHandler();
        // Debug.Log("File can be found at: " + Path.Combine(Application.persistentDataPath, fileName));
        LoadGame();
    }

    private void SetDataHandler() => dataHandler = new FileDataHandler<GameData>(Application.persistentDataPath, fileName);
    
    public void RegisterDataPersistenceObject(IDataPersistence dataPersistenceObj) => registeredObjects.Add(dataPersistenceObj);
    public void DeregisterDataPersistenceObject(IDataPersistence dataPersistenceObj) => registeredObjects.Remove(dataPersistenceObj);

    public void NewGame()
    {
        Debug.Log("Instatiating new Game Data");
        gameData = new GameData();
    }

    public void LoadGame()
    {
        // load any saved data from a file
        gameData = dataHandler.Load();

        // if no data can be loaded, initilise data
        if (gameData == null)
            NewGame();

        //Debug.Log("Loading game data");

        // push loaded data to all other scripts
        foreach (IDataPersistence dataPersistenceObj in registeredObjects)
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

        // pass the data to other scripts so they can update
        foreach (IDataPersistence dataPersistenceObj in registeredObjects)
            dataPersistenceObj.SaveData(ref gameData);

        // Debug.Log("Saving game data");

        // save that data to a file
        dataHandler.Save(gameData);

        Debug.Log("Saved game data");
    }

    /// <summary>
    /// <b>WARNING:</b> Only do this if you're sure you want to remove the existing data
    /// </summary>
    public void DestroySaveFile()
    {
        if (dataHandler == null) SetDataHandler();
        dataHandler?.Delete();
        Debug.Log("Deleted game data");
    }

    public bool HasSave()
    {
        return dataHandler?.Load() != null;
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("MainMenu"))
            SaveGame();
    }

    /// <summary>
    /// Registers all MonoBehaviours that have the type <see cref="IDataPersistence"/>
    /// </summary>
    /// <returns></returns>
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects =
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence>();

        return registeredObjects.Concat(dataPersistenceObjects).ToList();
    }
}