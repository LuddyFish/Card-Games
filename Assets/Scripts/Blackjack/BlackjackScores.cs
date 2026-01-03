using UnityEngine;
using UnityEngine.UI;

public class BlackjackScores : MonoBehaviour
{
    public PlayerObject playerObject;
    BlackjackPlayerState State => BlackjackGameManager.Instance.GetState(playerObject.data);

    private Text _numbers;
    private GameObject _bust;

    private bool _gameIsLoaded = false;

    private void Start()
    {
        _numbers = transform.Find("Numbers").GetComponent<Text>();
        _bust = transform.Find("Bust").gameObject;

        void ConfirmLoaded() => _gameIsLoaded = true;
        BlackjackGameManager.Instance.OnGameLoaded += ConfirmLoaded;
    }

    private void Update()
    {
        if (!_gameIsLoaded) return;
        
        Refresh();
    }

    /// <summary>
    /// Updates the text fields to match the Player's current <see cref="BlackjackPlayerState"/>
    /// </summary>
    public void Refresh()
    {
        _numbers.text = $"{State.Scores}\n{State.Wins}";
        _bust.SetActive(State.IsBust);
    }
}
