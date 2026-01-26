using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(AddEventAtRuntime))]
public class OnClickEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}

#endif

/// <summary>
/// This script is for <see cref="Button"/> <c>GameObject</c>s that need to have an <br/> 
/// event calling to <see cref="DataPersistenceManager"/>
/// </summary>
[RequireComponent(typeof(Button))]
public class AddEventAtRuntime : MonoBehaviour
{
    private DataPersistenceManager dataManager => DataPersistenceManager.Instance;
    private SceneSwitcher sceneSwitcher;
    private Button button;

    [Header("Data Add Requirements")]
    [SerializeField] private bool addEnterGameState = false;
    [Tooltip("0 = New Game\n1 = Resume Old Game"), SerializeField] private int enterGameState = 0;
    [Space(10)]
    [SerializeField] private bool addSaveAll = false;
    [Space(10)]
    [SerializeField] private bool goToNewScene = false;
    [SerializeField] private string sceneName = string.Empty;

    private void Start()
    {
        button = GetComponent<Button>();
        sceneSwitcher = FindFirstObjectByType<SceneSwitcher>();

        if (addEnterGameState) AddEnterGameState();
        if (addSaveAll) AddSaveAll();
        if (goToNewScene) SwitchScene(sceneName);
    }

    private void AddEnterGameState()
    {
        button.onClick.AddListener(SetGameState);
    }

    private void SetGameState()
    {
        dataManager.EnterGameState = enterGameState;
    }

    private void AddSaveAll()
    {
        button.onClick.AddListener(dataManager.SaveAll);
    }

    private void SwitchScene(string scene)
    {
        void GoScene() => sceneSwitcher.GoToScene(scene);
        button.onClick.AddListener(GoScene);
    }
}
