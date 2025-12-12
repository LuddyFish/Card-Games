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
            script.DestroySaveFile();
    }
}

#endif

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> DataPersistenceObjects => FindAllDataPersistenceObjects();
    private FileDataHandler dataHandler;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void Init()
    {
        SetDataHandler();
        Debug.Log("File can be found at: " + Path.Combine(Application.persistentDataPath, fileName));
        LoadGame();
    }

    private void SetDataHandler() => dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);

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

        Debug.Log("Loading game data");

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

        Debug.Log("Saving game data");

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
        dataHandler.Delete();
        Debug.Log("Deleted game data");
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = 
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
