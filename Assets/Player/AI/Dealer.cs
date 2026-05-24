using System.Collections;
using UnityEngine;

/// <summary>
/// The AI bot for the <c>Dealer</c>
/// </summary>
[RequireComponent(typeof(PlayerObject))]
public class Dealer : MonoBehaviour
{
    BlackjackGameManager Manager => BlackjackGameManager.Instance;
    PlayerObject me;

    [Min(0)]
    public float thinkingTime = 1f;
    private bool _performingAction = false;

    void Start()
    {
        me = GetComponent<PlayerObject>();
    }

    void Update()
    {
        if (!Manager.PlayersActive) return;
        if (me.data.isMyTurn && !_performingAction)
        {
            _performingAction = true;
            StartCoroutine(TryToWin());
        }
    }

    IEnumerator TryToWin()
    {
        yield return new WaitForSeconds(thinkingTime);
        if (HaveHighestScore())
        {
            Manager.Stay();
            Manager.TableHandler.RestPlayer(me.data); // Pre-initative rest
        }
        else
        {
            Manager.HitMe();
        }

        // Force rest if can't continue
        if (BlackjackScorer.GetPlayerScore(me) >= 21)
        {
            Manager.TableHandler.RestPlayer(me.data);
        }
        _performingAction = false;
    }

    bool HaveHighestScore()
    {
        int myScore = BlackjackScorer.GetPlayerScore(me);
        foreach (var player in Manager.Players)
        {
            int playerScore = BlackjackScorer.GetPlayerScore(player);
            if (playerScore > myScore && playerScore <= 21) return false;
        }

        return true;
    }
}
