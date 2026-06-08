using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerObject : MonoBehaviour, IDataPersistence
{
    [SerializeField] private CardGameContext _gameContext;
    Cardbox Box => Cardbox.Instance;
    CardAudio CardAudio => CardAudio.Instance;

    public Transform hand;
    public Transform jokerHand;

    public Player data;
    [HideInInspector] public List<CardObject> cards;
    [HideInInspector] public List<CardObject> jokers;

    [Tooltip("How long it takes for a card to move into hand")]
    public float collectTime = 0.5f;

    // -- Blackjack data --
    public int turnsWithoutJokers = 0;

    void OnDealHand() => SetHand(data.Hand, hand, cards);
    void OnDealJokers() => SetHand(data.Jokers, jokerHand, jokers);

    void Start()
    {
        data = new Player(name);
        cards = new List<CardObject>();
        jokers = new List<CardObject>();

        if (TryGetComponent<Dealer>(out _))
            _gameContext.ActiveGame.SetPlayer(this, 0);
        else
            _gameContext.ActiveGame.SetPlayer(this);

        _gameContext.ActiveGame.OnDeal += OnDealHand;
        _gameContext.ActiveGame.OnDeal += OnDealJokers;
        _gameContext.ActiveGame.OnReset += DiscardCards;

        data.OnTurnEnable += RevealHand;
    }

    public void LoadData(GameData data)
    {
        if (DataPersistenceManager.Instance == null) return;

        var cardsById = _gameContext.Deck.Cards.ToDictionary(c => c.Id);
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

    /// <summary>
    /// Finds it's position index in <see cref="Table.Players"/>
    /// </summary>
    /// <returns>Returns it's index position or <c>-1</c> if it can't</returns>
    public int GetPositionInTable()
    {
        for (int i = 0; i < _gameContext.Table.Players.Length; i++)
            if (data.ComparePlayer(_gameContext.Table.Players[i]))
                return i;
        return -1;
    }

    /// <summary>
    /// Retrieve all cards from <paramref name="source"/> and set the objects parents as <paramref name="parent"/>
    /// </summary>
    /// <param name="source">List of cards</param>
    /// <param name="parent">The hand that holds the cards</param>
    /// <param name="outputCache"></param>
    public void SetHand(List<Card> source, Transform parent, List<CardObject> outputCache)
    {
        // pre-emptive clear
        outputCache.Clear();
        var layout = parent.GetComponent<HandLayout>();

        // re-parent
        foreach (var card in source)
            if (_gameContext.CardMap.TryGetValue(card, out var obj))
                obj.transform.SetParent(parent);

        // receive
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            CardObject obj = child.GetComponent<CardObject>();
            obj.inHand = true;
            outputCache.Add(obj);
            layout.ReceiveCard(child, i, collectTime);
            child.GetComponent<SpriteRenderer>().sortingOrder = i;
        }
    }

    /// <summary>
    /// Removes a specified card from hand and gives it to <see cref="Cardbox"/>
    /// </summary>
    /// <param name="physical">The physical <c>GameObject</c> to return to <see cref="Cardbox"/></param>
    private void RemoveFromHand(Transform physical)
    {
        CardAudio.PlayCardDiscard(CardAudio.sources[Box.GetCardPosition(physical.gameObject) + 1]);
        Box.DiscardCard(physical);
    }

    /// <summary>
    /// Remove all cards from <see cref="data"/> and <see cref="hand"/>
    /// </summary>
    public void DiscardCards()
    {
        data.Hand.Clear();
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
        foreach (var card in cards)
            card.Reveal();
    }

    void OnDestroy()
    {
        if (_gameContext.ActiveGame != null)
        {
            _gameContext.ActiveGame.OnDeal -= OnDealHand;
            _gameContext.ActiveGame.OnDeal -= OnDealJokers;
            _gameContext.ActiveGame.OnReset -= DiscardCards;
        }

        data.OnTurnEnable -= RevealHand;
    }
}
