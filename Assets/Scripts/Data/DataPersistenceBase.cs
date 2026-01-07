using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public abstract class DataPersistenceBase : MonoBehaviour
{
    public abstract void Init();
    public abstract void LoadAll();
    public abstract void SaveAll();
    public abstract void DestroyAll();

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public virtual void New<T>(DataComponent<T> component) where T : class, new()
    {
        Debug.Log($"Instatiating new {typeof(T).Name}");
        component.New();
    }

    public virtual void Load<T>(DataComponent<T> component, List<IDataPersistence<T>> dataPersistenceObjects) where T : class, new()
    {
        component.Load();
        foreach (var dataPersistenceObj in dataPersistenceObjects)
            dataPersistenceObj.LoadData(component.data);

        Debug.Log($"Loaded {typeof(T).Name}");
    }

    public virtual void Save<T>(DataComponent<T> component, List<IDataPersistence<T>> dataPersistenceObjects) where T : class, new()
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
    public virtual void DestroySaveFile<T>(FileDataHandler<T> handler) where T : class
    {
        handler?.Delete();

        Debug.Log($"Deleted {typeof(T).Name} save data");
    }

    protected virtual void OnApplicationQuit()
    {
        SaveAll();
    }

    protected List<IDataPersistence<T>> FindAllDataPersistenceObjects<T>() where T : class
    {
        IEnumerable<IDataPersistence<T>> dataPersistenceObjects =
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence<T>>();

        return new List<IDataPersistence<T>>(dataPersistenceObjects);
    }
}

[System.Serializable]
public class DataComponent<T> where T : class, new()
{
    [SerializeField] private string _fileName;
    public string FilePath =>
        Path.Combine(Application.persistentDataPath, _fileName);

    [HideInInspector] public T data;
    public FileDataHandler<T> handler;

    public void SetHandler() =>
        handler = new FileDataHandler<T>
        (Application.persistentDataPath, _fileName);

    public void New() =>
        data = new T();

    public void Load()
    {
        data = handler.Load();
        if (data == null)
            New();
    }

    public void Save() =>
        handler.Save(data);
}