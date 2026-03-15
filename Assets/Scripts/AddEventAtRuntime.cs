using UnityEngine;
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

        var ARG = EditorGUILayout.Toggle("Add Resume Game", script.resumeGame);
        script.resumeGame = ARG;

        EditorGUILayout.Space();

        var ASG = EditorGUILayout.Toggle("Add Save Game", script.addSaveGame);
        script.addSaveGame = ASG;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Scene Switch", EditorStyles.boldLabel);

        #region Scenes
        EditorGUIUtility.labelWidth = 100;
        float width = EditorGUIUtility.currentViewWidth / 3f;
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        var GTNS = EditorGUILayout.Toggle("Go To New Scene", script.goToNewScene, GUILayout.Width(width));
        if (EditorGUI.EndChangeCheck())
        {
            script.goToNewScene = GTNS;
            if (GTNS)
            {
                script.addScene = false;
                script.removeScene = false;
            }
        }

        EditorGUI.BeginChangeCheck();
        var AS = EditorGUILayout.Toggle("Add Scene", script.addScene, GUILayout.Width(width));
        if (EditorGUI.EndChangeCheck())
        {
            script.addScene = AS;
            if (AS)
            {
                script.goToNewScene = false;
                script.removeScene = false;
            }
        }

        EditorGUI.BeginChangeCheck();
        var RS = EditorGUILayout.Toggle("Remove Scene", script.removeScene, GUILayout.Width(width));
        if (EditorGUI.EndChangeCheck())
        {
            script.removeScene = RS;
            if (RS)
            {
                script.goToNewScene = false;
                script.addScene = false;
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUIUtility.labelWidth = 0;

        if ( GTNS || AS || RS )
        {
            EditorGUILayout.PropertyField(sceneField);
        }
        #endregion

        serializedObject.ApplyModifiedProperties();
    }
}

#endif

/// <summary>
/// This script is for <see cref="Button"/> <c>GameObject</c>s that need to have an <br/> 
/// event calling to <see cref="DataPersistenceManager"/><br/>
/// This script also ensures that <c>Scene Switches</c> occur last to ensure all other<br/>
/// events have occured that are reliant on the current active scene
/// </summary>
[RequireComponent(typeof(Button))]
public class AddEventAtRuntime : MonoBehaviour
{
    private DataPersistenceManager DataManager => DataPersistenceManager.Instance;
    private SceneSwitcher sceneSwitcher;
    private Button button;

    [Header("Data Add Requirements")]
    [Tooltip("0 = New Game | 1 = Resume Old Game")] public bool resumeGame = false;
    [Space(10)]
    public bool addSaveGame = false;

    [Header("Scene Switch")]
    public bool goToNewScene = false;
    public bool addScene = false;
    public bool removeScene = false;
    public SceneField sceneToLoad;

    private void Start()
    {
        button = GetComponent<Button>();
        sceneSwitcher = FindFirstObjectByType<SceneSwitcher>();

        AddEnterGameState();
        if (addSaveGame) AddSave();

        if (goToNewScene) SwitchScene(sceneToLoad);
        else if (addScene) AddScene(sceneToLoad);
        else if (removeScene) RemoveScene(sceneToLoad);
    }

    private void AddEnterGameState()
    {
        button.onClick.AddListener(SetGameState);
    }

    private void SetGameState()
    {
        if (DataManager != null)
            DataManager.ResumeGame = resumeGame;
    }

    private void AddSave()
    {
        button.onClick.AddListener(() => DataManager.SaveGame());
    }

    private void SwitchScene(SceneField scene)
    {
        button.onClick.AddListener(
            () => sceneSwitcher.GoToScene(scene)
        );
    }

    private void AddScene(SceneField scene)
    {
        button.onClick.AddListener(
            () => sceneSwitcher.AddScene(scene)
        );
    }

    private void RemoveScene(SceneField scene)
    {
        button.onClick.AddListener(
            () => sceneSwitcher.UnloadScene(scene)
        );
    }
}
