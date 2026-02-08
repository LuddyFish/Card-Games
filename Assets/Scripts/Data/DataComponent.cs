using System;
using System.IO;
using UnityEngine;

public class DataComponent<T> : MonoBehaviour, IDataComponent where T : class, new()
{
    [SerializeField] private string _fileName;

    public T Data { get; private set; }
    public string FilePath => 
        Path.Combine(Application.persistentDataPath, _fileName);
    public Type DataType => typeof(T);

    private FileDataHandler<T> _handler;

    private void Awake()
    {
        SetHandler();
        DataPersistenceManager.Instance.Register(this);
    }

    private void SetHandler() =>
        _handler = new FileDataHandler<T>
        (Application.persistentDataPath, _fileName);

    public void New() =>
        Data = new T();

    public void Load()
    {
        Data = _handler.Load();
        if (Data == null)
            New();
    }

    public void Save() =>
        _handler.Save(Data);
}
