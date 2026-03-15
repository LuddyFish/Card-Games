using UnityEngine;

public static class InitialiseOnLoad
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        Debug.Log("Loaded by Persist Objects from \"InitialiseOnLoad\"");
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("PERSISTOBJECTS")));
    }
}
