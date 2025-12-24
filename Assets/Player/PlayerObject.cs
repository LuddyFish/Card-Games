using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerObject : MonoBehaviour, IDataPersistence<GameData>
{
    BlackjackGameManager BJGM => BlackjackGameManager.Instance;
    Cardbox Box => Cardbox.Instance;
    CardAudio CardAudio => CardAudio.Instance;

    private Transform _hand;

    public Player data;
    [HideInInspector] public List<CardObject> cards;

    [Tooltip("How long it takes for a card to move into hand")]
    public float collectTime = 0.5f;
    private bool handRevealed = false;

    void Start()
    {
        _hand = transform.Find("Hand").transform;
        data = new Player(name);
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
        var cardsById = Deck.Cards.ToDictionary(c => c.Id);
        foreach (var player in data.players)
            if (this.data.ComparePlayer(player.id))
                player.TransferData(this.data, cardsById);
    }

    public void SaveData(ref GameData data)
    {
        int index = GetPositionInTable();
        if (index == -1)
        {
            Debug.LogError($"Could not find player: \"{name}\" id in Table");
            return;
        }

        data.players[index] = new(this.data);
    }

    void Update()
    {
        if (data.isMyTurn && !handRevealed)
            RevealHand();
        else if (!data.isMyTurn)
            handRevealed = false;
    }

    /// <summary>
    /// Finds it's position index in <see cref="Table.Players"/>
    /// </summary>
    /// <returns>Returns it's index position or <c>-1</c> if it can't</returns>
    public int GetPositionInTable()
    {
        for (int i = 0; i < Table.Players.Length; i++)
            if (data.ComparePlayer(Table.Players[i]))
                return i;
        return -1;
    }

    /// <summary>
    /// Retrieve all cards in <see cref="data"/> and add it to <see cref="_hand"/><br/>
    /// Also resets <see cref="_hand"/> to avoid mistakes
    /// </summary>
    public void SetHand()
    {
        var objsById = Box.cards
            .Select(obj => obj.GetComponent<CardObject>())
            .ToDictionary(obj => obj.card.Id);

        foreach (var card in data.Hand)
            if (objsById.TryGetValue(card.Id, out var obj))
                obj.transform.SetParent(_hand);

        SetCards();
    }

    /// <summary>
    /// Set all cards in <see cref="_hand"/> to <see cref="cards"/>
    /// </summary>
    private void SetCards()
    {
        cards.Clear(); // Pre-emptive removal to avoid dupliactes

        var layout = _hand.GetComponent<HandLayout>();
        for (int i = 0; i < _hand.childCount; i++)
        {
            Transform child = _hand.GetChild(i);
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
    /// Remove all cards from <see cref="data"/> and <see cref="_hand"/>
    /// </summary>
    public void DiscardCards()
    {
        data.Hand.Clear();
        while (_hand.childCount > 0)
            RemoveFromHand(_hand.GetChild(0));
        foreach (var card in cards)
            card.Hide();
        cards.Clear();
    }

    /// <summary>
    /// Turn all cards in <see cref="_hand"/> face up
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
