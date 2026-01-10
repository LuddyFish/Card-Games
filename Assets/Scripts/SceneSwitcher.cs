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

    public void AddScene(string name)
    {
        SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
    }

    public void AddScene(int index)
    {
        SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
    }

    public void GoToAsyncScene(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }

    public void GoToAsyncScene(int index)
    {
        SceneManager.LoadSceneAsync(index);
    }

    public void UnloadScene(string name)
    {
        SceneManager.UnloadSceneAsync(SceneUtilities.GetScene(name));
    }

    public void UnloadScene(int index)
    {
        SceneManager.UnloadSceneAsync(SceneUtilities.GetScene(index));
    }

    public void Quit()
    {
        SceneUtilities.Quit();
    }
}
