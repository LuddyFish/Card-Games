using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void GoToScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void GoToScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void GoToScene(SceneField scene)
    {
        SceneManager.LoadScene(scene.SceneName);
    }

    public void AddScene(string name)
    {
        SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
    }

    public void AddScene(int index)
    {
        SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
    }

    public void AddScene(SceneField scene)
    {
        SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive);
    }

    public void GoToAsyncScene(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }

    public void GoToAsyncScene(int index)
    {
        SceneManager.LoadSceneAsync(index);
    }

    public void GoToAsyncScene(SceneField scene)
    {
        SceneManager.LoadSceneAsync(scene.SceneName);
    }

    public void UnloadScene(string name)
    {
        SceneManager.UnloadSceneAsync(SceneUtilities.GetScene(name));
    }

    public void UnloadScene(int index)
    {
        SceneManager.UnloadSceneAsync(SceneUtilities.GetScene(index));
    }

    public void UnloadScene(SceneField scene)
    {
        SceneManager.UnloadSceneAsync(SceneUtilities.GetScene(scene.SceneName));
    }

    public void Quit()
    {
        SceneUtilities.Quit();
    }
}
