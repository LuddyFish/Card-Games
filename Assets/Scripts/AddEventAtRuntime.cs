using UnityEngine;
using UnityEngine.UI;

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
    
    public enum SceneAction { None, Goto, Add, Remove }
    [Header("Scene Switch")]
    public SceneAction action = SceneAction.None;
    public SceneField sceneToLoad;

    private void Start()
    {
        button = GetComponent<Button>();
        sceneSwitcher = FindFirstObjectByType<SceneSwitcher>();

        AddEnterGameState();
        if (addSaveGame) AddSave();

        switch (action)
        {
            case SceneAction.None: break;
            case SceneAction.Goto: SwitchScene(sceneToLoad); break;
            case SceneAction.Add: AddScene(sceneToLoad); break;
            case SceneAction.Remove: RemoveScene(sceneToLoad); break;
        }
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
            () => sceneSwitcher.GoToScene(scene.SceneName)
        );
    }

    private void AddScene(SceneField scene)
    {
        button.onClick.AddListener(
            () => sceneSwitcher.AddScene(scene.SceneName)
        );
    }

    private void RemoveScene(SceneField scene)
    {
        button.onClick.AddListener(
            () => sceneSwitcher.UnloadScene(scene.SceneName)
        );
    }
}
