using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    private GameData gameData;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        // TODO: load any saved data from a file

        // if no data can be loaded, initilise data
        if (gameData == null)
            NewGame();
    }

    public void SaveGame()
    {
        // TODO: pass the data to other scripts so they can update

        // TODO: save that data to a file
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
