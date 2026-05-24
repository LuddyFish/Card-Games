using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject _pausedScreen;
    [HideInInspector] public bool isPaused = false;

    public void TogglePause(bool pause)
    {
        isPaused = pause;
        SetPause();
    }

    private void SetPause()
    {
        _pausedScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }
}
