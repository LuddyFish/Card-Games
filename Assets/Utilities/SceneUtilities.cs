using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUtilities
{
    public static void NextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
            SceneManager.LoadScene(nextSceneIndex);
        else
            SceneManager.LoadScene(0);
    }

    public static Scene GetScene(string name)
    {
        var scene = SceneManager.GetSceneByName(name);
        if (!scene.IsValid())
        {
            Debug.LogError($"Could not find scene of name: {name}");
            return default;
        }
        return scene;
    }

    public static Scene GetScene(int index)
    {
        var scene = SceneManager.GetSceneByBuildIndex(index);
        if (!scene.IsValid())
        {
            Debug.LogError($"Could not find scene at index: {index}");
            return default;
        }
        return scene;
    }

    public static void Quit()
    {
        Application.Quit();
    }
}
