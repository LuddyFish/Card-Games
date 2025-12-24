using UnityEngine;
using UnityEngine.UI;

public class BlackjackScores : MonoBehaviour
{
    BlackjackGameManager BJGM => BlackjackGameManager.Instance;

    private Text _numbers;
    private GameObject _bust;

    private int _score = 0;
    public int Scores
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            UpdateText();
            if (_score > 21) ToggleBust(true);
        }
    }
    private int _wins = 0;
    public int Wins
    {
        get
        {
            return _wins;
        }
        set
        {
            _wins = value;
            UpdateText();
        }
    }

    void Start()
    {
        _numbers = transform.Find("Numbers").GetComponent<Text>();
        _bust = transform.Find("Bust").gameObject;
        UpdateText();

        if (name.Contains("Dealer"))
            BJGM.SetScorer(this, 0);
        else
            BJGM.SetScorer(this);
    }

    private void UpdateText()
    {
        _numbers.text = $"{_score}\n{_wins}";
    }

    public void ToggleBust(bool? cond)
    {
        _bust.SetActive(cond ?? !_bust.activeSelf);
    }
}
