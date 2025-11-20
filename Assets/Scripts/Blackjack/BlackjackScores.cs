using UnityEngine;
using UnityEngine.UI;

public class BlackjackScores : MonoBehaviour
{
    BlackjackGameManager BJGM => BlackjackGameManager.Instance;

    private Text description;
    private GameObject bust;

    private int score = 0;
    private int wins = 0;

    void Start()
    {
        description = transform.Find("Description").GetComponent<Text>();
        bust = transform.Find("Bust").gameObject;
        UpdateText();

        if (name.Contains("Dealer"))
            BJGM.SetScorer(this, 0);
        else
            BJGM.SetScorer(this);
    }

    private void UpdateText()
    {
        description.text = $"Score:\t\t\t\t\t\t{score}\n" +
            $"Wins:\t\t\t\t\t\t{wins}";
    }

    public int GetScore()
    {
        return score;
    }

    public void SetScore(int score)
    {
        this.score = score;
        UpdateText();
        if (this.score > 21) ToggleBust(true);
    }

    public int GetWins()
    {
        return wins;
    }

    public void SetWins(int wins)
    {
        this.wins = wins;
        UpdateText();
    }

    public void ToggleBust(bool? cond)
    {
        bust.SetActive(cond ?? !bust.activeSelf);
    }
}
