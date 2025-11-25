using UnityEngine;
using UnityEngine.UI;

public class BlackjackScores : MonoBehaviour
{
    BlackjackGameManager BJGM => BlackjackGameManager.Instance;

    private Text numbers;
    private GameObject bust;

    private int score = 0;
    private int wins = 0;

    void Start()
    {
        numbers = transform.Find("Numbers").GetComponent<Text>();
        bust = transform.Find("Bust").gameObject;
        UpdateText();

        if (name.Contains("Dealer"))
            BJGM.SetScorer(this, 0);
        else
            BJGM.SetScorer(this);
    }

    private void UpdateText()
    {
        numbers.text = $"{score}\n{wins}";
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
