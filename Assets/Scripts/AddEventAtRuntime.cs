using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(AddEventAtRuntime))]
public class OnClickEditor : Editor
{
    SerializedProperty sceneField;

    private void OnEnable()
    {
        sceneField = serializedObject.FindProperty("sceneToLoad");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        AddEventAtRuntime script = (AddEventAtRuntime)target;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
        GUI.enabled = true;

        EditorGUILayout.LabelField("Data Add Requirements", EditorStyles.boldLabel);

        var AEGS = EditorGUILayout.Toggle("Add Enter Game State", script.addEnterGameState);
        script.addEnterGameState = AEGS;
        if ( AEGS )
        {
            var EGS = EditorGUILayout.IntField(
                new GUIContent("Enter Game State", "0 = New Game\n1 = Resume Old Game"), 
                script.enterGameState
            );
            script.enterGameState = EGS;
        }

        EditorGUILayout.Space();

        var ASA = EditorGUILayout.Toggle("Add Save All", script.addSaveAll);
        script.addSaveAll = ASA;

        EditorGUILayout.Space();

        var GTNS = EditorGUILayout.Toggle("Go To New Scene", script.goToNewScene);
        script.goToNewScene = GTNS;
        if ( GTNS )
        {
            EditorGUILayout.PropertyField(sceneField);
        }

        serializedObject.ApplyModifiedProperties();
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
    public bool addEnterGameState = false;
    [Tooltip("0 = New Game\n1 = Resume Old Game")] public int enterGameState = 0;
    [Space(10)]
    public bool addSaveAll = false;
    [Space(10)]
    public bool goToNewScene = false;
    public SceneField sceneToLoad;

    private void Start()
    {
        button = GetComponent<Button>();
        sceneSwitcher = FindFirstObjectByType<SceneSwitcher>();

        if (addEnterGameState) AddEnterGameState();
        if (addSaveAll) AddSaveAll();
        if (goToNewScene) SwitchScene(sceneToLoad);
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

    private void SwitchScene(SceneField scene)
    {
        void GoScene() => sceneSwitcher.GoToScene(scene);
        button.onClick.AddListener(GoScene);
    }
}
