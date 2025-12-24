using System.IO;
using UnityEngine;

public abstract class DataPersistenceBase : MonoBehaviour
{
    public abstract void Init();
    protected abstract void LoadAll();
    protected abstract void SaveAll();
    protected abstract void DestroyAll();

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
    }

    protected virtual void OnApplicationQuit()
    {
        SaveAll();
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