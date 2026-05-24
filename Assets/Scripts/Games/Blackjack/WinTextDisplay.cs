using UnityEngine;
using UnityEngine.UI;

public class WinTextDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _winTextBox;
    private Text winText;

    private void Start()
    {
        winText = GetComponent<Text>();
        HideWinText();
    }

    /// <summary>
    /// Sets the Player's name on the <c>Winner</c> text box
    /// </summary>
    /// <param name="player"></param>
    public void DisplayWinner(PlayerObject player)
    {
        winText.text = player != null ? $"{player.name} Wins!" : "Everyone's bust...";
        _winTextBox.SetActive(true);
    }

    public void HideWinText()
    {
        _winTextBox.SetActive(false);
    }
}
