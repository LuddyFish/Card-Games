using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour, IDataPersistence
{
    private BlackjackGameManager BJGM => BlackjackGameManager.Instance;
    private Cardbox Box => Cardbox.Instance;
    private CardAudio CardAudio => CardAudio.Instance;
    Transform hand;

    public Player Data { get; private set; }
    [HideInInspector] public List<CardObject> cards;

    [Tooltip("How long it takes for a card to move into hand")]
    public float collectTime = 0.5f;
    private bool handRevealed = false;

    void Start()
    {
        hand = transform.Find("Hand").transform;
        Data = new Player(name);
        cards = new List<CardObject>();

        if (TryGetComponent<Dealer>(out _))
            BJGM?.SetPlayer(this, 0);
        else
            BJGM?.SetPlayer(this);
        BJGM.onDeal += SetHand;
        BJGM.onReset += DiscardCards;
    }

    public void LoadData(GameData data)
    {

    }

    public void SaveData(ref GameData data)
    {
        int index = GetPositionInTable();
        if (index == -1)
        {
            Debug.LogError($"Could not find player: \"{name}\" id in Table");
            return;
        }

        data.Players[index] = new(Data);
    }

    void Update()
    {
        if (Data.isMyTurn && !handRevealed)
            RevealHand();
        else if (!Data.isMyTurn)
            handRevealed = false;
    }

    /// <summary>
    /// Finds it's position index in <see cref="Table.Players"/>
    /// </summary>
    /// <returns>Returns it's index position or <c>-1</c> if it can't</returns>
    public int GetPositionInTable()
    {
        for (int i = 0; i < Table.Players.Length; i++)
            if (Data.ComparePlayer(Table.Players[i]))
                return i;
        return -1;
    }

    /// <summary>
    /// Retrieve all cards in <see cref="Data"/> and add it to <see cref="hand"/><br/>
    /// Also resets <see cref="hand"/> to avoid mistakes
    /// </summary>
    public void SetHand()
    {
        foreach (var card in Data.Hand)
            foreach (var obj in Box.cards)
                if (obj.name == card.GetName())
                {
                    obj.transform.SetParent(hand);
                    break;
                }
        SetCards();
    }

    /// <summary>
    /// Set all cards in <see cref="hand"/> to <see cref="cards"/>
    /// </summary>
    private void SetCards()
    {
        cards.Clear(); // Pre-emptive removal to avoid dupliactes

        var layout = hand.GetComponent<HandLayout>();
        for (int i = 0; i < hand.childCount; i++)
        {
            Transform child = hand.GetChild(i);
            CardObject obj = child.GetComponent<CardObject>();
            obj.inHand = true;
            cards.Add(obj);
            layout.ReceiveCard(child, i, collectTime);
            child.GetComponent<SpriteRenderer>().sortingOrder = i; // Ensure that there's a layering
        }
    }

    /// <summary>
    /// Removes a specified card from hand and gives it to <see cref="Cardbox"/>
    /// </summary>
    /// <param name="physical">The physical <c>GameObject</c> to return to <see cref="Cardbox"/></param>
    private void RemoveFromHand(Transform physical)
    {
        CardAudio.Play(
            CardAudio.sources[Box.GetCardPosition(physical.gameObject) + 1],
            CardAudio.audios[0]
        );
        Box.DiscardCard(physical);
    }

    /// <summary>
    /// Remove all cards from <see cref="Data"/> and <see cref="hand"/>
    /// </summary>
    public void DiscardCards()
    {
        Data.Hand.Clear();
        while (hand.childCount > 0)
            RemoveFromHand(hand.GetChild(0));
        foreach (var card in cards)
            card.Hide();
        cards.Clear();
    }

    /// <summary>
    /// Turn all cards in <see cref="hand"/> face up
    /// </summary>
    public void RevealHand()
    {
        handRevealed = true;
        foreach (var card in cards)
        {
            card.Reveal();
        }
    }

    /// <summary>
    /// Calculates my score
    /// </summary>
    /// <returns>The sum value of all my cards</returns>
    public int GetScore()
    {
        return BJGM.GetPlayerScore(this);
    }
}
