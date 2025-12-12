using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private readonly string dataDirPath = string.Empty;
    private readonly string dataFileName = string.Empty;
    private readonly string dataSubFolderPath = string.Empty;

    private string fullPath => Path.Combine(dataDirPath, dataSubFolderPath, dataFileName);

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.dataSubFolderPath = string.Empty;
    }

    public FileDataHandler(string dataDirPath, string dataFileName, string dataSubFolderPath)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.dataSubFolderPath = dataSubFolderPath;
    }

    public GameData Load()
    {
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                // Load serialized data from file
                using FileStream stream = new FileStream(fullPath, FileMode.Open);
                using StreamReader reader = new StreamReader(stream);
                string dataToLoad = reader.ReadToEnd();

                // Deserialize data from Json
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error occured when trying to load data from file: {fullPath}\n{e}");
            }
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        try
        {
            // Create or get directory
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize game data to Json
            string dataToStore = JsonUtility.ToJson(data, true);
            using FileStream stream = new FileStream(fullPath, FileMode.Create);
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write(dataToStore);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occured when trying to save data to file: {fullPath}\n{e}");
        }
    }

    public void Delete()
    {
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
