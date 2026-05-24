using UnityEngine;

public class JokerBank : MonoBehaviour
{
    public static JokerBank Instance { get; private set; }
    [SerializeField] private CardGameContext _context;

    [Tooltip("How many turns to wait until you receive a joker when you have none")]
    public int jokerRefreshRound = 5;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void GiftJoker(PlayerObject player)
    {
        var joker = Cardbox.Instance.GetAvailableJoker().GetComponent<CardObject>().card;
        _context.Deck.DealSpecific(player.data, joker);
    }

    public void GiftPityJoker(PlayerObject player)
    {
        if (player.jokers.Count > 0)
        {
            player.turnsWithoutJokers = 0;
            return;
        }

        if (player.turnsWithoutJokers < jokerRefreshRound)
        {
            player.turnsWithoutJokers++;
        }
        else
        {
            player.turnsWithoutJokers = 0;
            GiftJoker(player);
        }
    }
}
