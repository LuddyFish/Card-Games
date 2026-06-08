using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class <see cref="Deck"/> is responsible for maintaining the cards in the deck
/// </summary>
public class Deck : IDataPersistence
{
    /// <summary>
    /// Whole deck of cards. This should not be modified!
    /// </summary>
    public Card[] Cards { get; private set; }
    /// <summary>
    /// Sudo deck of cards that tracks whether or not cards are available<br/>
    /// to be drawn from the deck or not
    /// </summary>
    private readonly List<Card> _pool;

    // How many suits and ranks
    private readonly int _suitCount;
    private readonly int _rankCount;
    private readonly Func<Card, bool> _isAvailable;

    public event Action<Player, Card> OnCardDealt;
    public event Action OnDeckShuffled;
    public event Action OnDeckSoftShuffled;
    public event Action OnBatchDealStart;

    public Deck(int suitCount = 4, int rankCount = 13, CardDeckSet set = null, Func<Card, bool> isAvailable = null)
    {
        _suitCount = suitCount;
        _rankCount = rankCount;
        if (set == null || set.cards.Count == 0)
        {
            Cards = new Card[_suitCount * _rankCount];
            for (int s = 0; s < _suitCount; s++)
                for (int r = 1; r <= _rankCount; r++)
                    Cards[s * _rankCount + r - 1] = new Card(s, r, s * _rankCount + r);
        }
        else
        {
            Cards = new Card[set.cards.Count];
            for (int i = 0; i < set.cards.Count; i++)
                Cards[i] = new(set.cards[i].suit, set.cards[i].rank, i);
        }
        _pool = new();
        _isAvailable = isAvailable;
    }

    public void LoadData(GameData data)
    {
        if (DataPersistenceManager.Instance == null) return;
        if (data.cards == null || data.cards.Count == 0) return;
        
        Cards = data.LoadCards(Cards.ToDictionary(c => c.Id));
    }

    public void SaveData(ref GameData data)
    {
        data.SaveDeckData(this);
    }

    /// <summary>
    /// Reset the cards in the pool to choose from
    /// </summary>
    public void NewDeck()
    {
        _pool.Clear();
        foreach (Card card in Cards)
            _pool.Add(card);
        OnDeckShuffled?.Invoke();
    }

    /// <summary>
    /// Reset the cards in the pool to choose from but leaves all cards 
    /// </summary>
    public void NewSoftDeck()
    {
        _pool.Clear();
        foreach (var card in Cards)
            if (_isAvailable == null || _isAvailable(card))
                _pool.Add(card);
        OnDeckSoftShuffled?.Invoke();
    }

    /// <summary>
    /// Find the specified card in the deck
    /// </summary>
    /// <param name="card"></param>
    /// <returns>Index position of card</returns>
    private int FindCardIndex(Card card)
    {
        for (int i = 0; i < Cards.Length; i++)
            if (Cards[i] == card)
                return i;
        return -1;
    }

    /// <summary>
    /// Get a random card from <see cref="_pool"/>
    /// </summary>
    /// <returns>Returns a random card in <see cref="_pool"/></returns>
    private Card GetRandomCard()
    {
        int index = UnityEngine.Random.Range(0, _pool.Count);
        return _pool.ElementAt(index);
    }

    /// <summary>
    /// Get a random card in <see cref="_pool"/> and then remove it
    /// </summary>
    /// <returns>A card</returns>
    public Card DealRandomCard()
    {
        if (IsDeckEmpty()) NewSoftDeck(); // Ensures that can deal cards.
        int index = UnityEngine.Random.Range(0, _pool.Count);
        Card card = _pool.ElementAt(index);
        _pool.RemoveAt(index);
        return card;
    }

    /// <summary>
    /// Deal a batch of cards all at once
    /// </summary>
    /// <param name="table">The game Table which to deal to</param>
    /// <param name="fillHands">Does the deck reshuffle if there were not enough cards?</param>
    public void DealContinuous(Table table, bool fillHands = false)
    {
        var dealer = table.GetDealer();
        int cardsDealt = 0;
        bool CardsNeedDealing() => cardsDealt < table.Players.Length * table.StartingCardCount;
        void RestockDeck()
        {
            if (IsDeckEmpty() && CardsNeedDealing() && fillHands)
                NewSoftDeck();
        }

        OnBatchDealStart?.Invoke();
        RestockDeck();
        do
        {
            int playerIndex = (cardsDealt + dealer + 1) % table.Players.Length;
            Card card = GetRandomCard();
            table.Players[playerIndex].Hand.Add(card);
            Cards[FindCardIndex(card)].inPlay = true;
            _pool.Remove(card);
            cardsDealt++;
            RestockDeck();
        } while (CardsNeedDealing());
    }

    /// <summary>
    /// Deal a singular card (used as part of a segment)
    /// </summary>
    /// <param name="table">The game Table which to deal to</param>
    public void DealSegmented(Table table, int cardDealIndex)
    {
        var playerToDealIndex = (table.GetDealer() + cardDealIndex + 1) % table.Players.Length;
        var player = table.Players[playerToDealIndex];
        var card = DealRandomCard();
        
        player.Hand.Add(card);
        OnCardDealt?.Invoke(player, card);
    }

    public void DealSpecific(Player player, Card card)
    {
        if (card.Rank != (int)Ranks.joker)
            player.Hand.Add(card);
        else
            player.Jokers.Add(card);

        OnCardDealt?.Invoke(player, card);
    }

    /// <summary>
    /// Check how many cards remain in <see cref="_pool"/>
    /// </summary>
    /// <returns>Returns <c>True</c> if no cards remain in <see cref="_pool"/></returns>
    public bool IsDeckEmpty() => _pool.Count <= 0;

    /// <summary>
    /// Check if there is enough remaining number of cards in <see cref="_pool"/> <br/>
    /// without having to reshuffle cards back into <see cref="_pool"/>
    /// </summary>
    /// <param name="cardsToDeal">Number of cards that need to be dealt</param>
    /// <returns>Returns <c>True</c> if not enough cards remain in <see cref="_pool"/></returns>
    public bool NotEnoughCards(int cardsToDeal) => _pool.Count < cardsToDeal;
}
