using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void GoToScene(string name)
    {
        SceneUtilities.LoadScene(name);
    }

    public void GoToScene(int index)
    {
        SceneUtilities.LoadScene(index);
    }

    public void Quit()
    {
        SceneUtilities.Quit();
    }
}
