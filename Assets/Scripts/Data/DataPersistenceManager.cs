using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    private readonly Dictionary<Type, IDataComponent> _components = new();

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

    public void Register(IDataComponent component)
    {
        _components[component.DataType] = component;
    }

    public void Init()
    {
        foreach (IDataComponent component in _components.Values)
        {
            if (_resumeGame)
                Load(component);
            else
                New(component);
        }
    }

    public void SaveAll()
    {
        foreach (IDataComponent component in _components.Values)
        {
            Save(component);
        }
    }

    public void New(IDataComponent component)
    {
        component.New();
    }

    public void Load(IDataComponent component)
    {
        component.Load();

        var persistenceObjects = FindAllPersistenceObjects(component.DataType);
        foreach (var dataObj in persistenceObjects)
            dataObj.GetType()
                .GetMethod("LoadData")
                .Invoke(dataObj, new object[] { 
                    GetComponentData(component) 
                });
    }

    public void Save(IDataComponent component)
{
        var persistenceObjects = FindAllPersistenceObjects(component.DataType);

        object data = GetComponentData(component);
        foreach (var dataObj in persistenceObjects)
        {
            object[] parameters = { data };
            dataObj.GetType()
                .GetMethod("SaveData")
                .Invoke(dataObj, parameters);
            data = parameters[0];
        }

        SetComponentData(component, data);

        component.Save();
    }

    private object GetComponentData(IDataComponent component)
    {
        return component.GetType().GetProperty("Data").GetValue(component);
    }

    private void SetComponentData(IDataComponent component, object data)
    {
        component.GetType().GetProperty("Data").SetValue(component, data);
    }

    public bool HasSave<T>() where T : class, new()
    {
        if (_components.TryGetValue(typeof(T), out var component))
            return File.Exists((component as DataComponent<T>).FilePath);

        return false;
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("MainMenu"))
            SaveAll();
    }

    private List<object> FindAllPersistenceObjects(Type dataType)
    {
        var target = typeof(IDataPersistence<>).MakeGenericType(dataType);

        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(obj => target.IsAssignableFrom(obj.GetType()))
            .Cast<object>()
            .ToList();
    }
}