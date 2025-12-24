using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class <see cref="Deck"/> is responsible for maintaining the cards in the deck
/// </summary>
public class Deck
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
    private readonly int _suitCount = 4;
    private readonly int _rankCount = 13;

    Cardbox Box => Cardbox.Instance;
    CardAudio Audio => CardAudio.Instance;

    public Deck(int? suitCount = null, int? rankCount = null)
    {
        _suitCount = suitCount ?? _suitCount;
        _rankCount = rankCount ?? _rankCount;
        Cards = new Card[_suitCount * _rankCount];
        for (int s = 0; s < _suitCount; s++)
            for (int r = 1; r <= _rankCount; r++)
                Cards[s * _rankCount + r - 1] = new Card(s, r, s * _rankCount + r);
        _pool = new();
    }

    /// <summary>
    /// Reset the cards in the pool to choose from.
    /// </summary>
    public void NewDeck()
    {
        PlayCardSound(Audio.sources[0], 3);
        _pool.Clear();
        foreach (Card card in Cards)
            _pool.Add(card);
    }

    /// <summary>
    /// Reset the cards in the pool to choose from but leaves all cards 
    /// </summary>
    public void NewSoftDeck()
    {
        if (Box != null)
        {
            PlayCardSound(Audio.sources[0], 3);
            _pool.Clear();
            for (int i = 0; i < Cards.Length; i++)
            {
                var card = Box.cards[i].GetComponent<CardObject>();
                if (!card.inHand)
                {
                    _pool.Add(card.card);
                    Box.ReturnCard(Box.cards[i].transform);
                }
            }
        }
        else
            NewDeck();
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
        return 0;
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
    /// Get a random card in <see cref="_pool"/>
    /// </summary>
    /// <returns>A card</returns>
    public Card DealRandomCard()
    {
        if (IsDeckEmpty()) NewSoftDeck(); // Ensures that can deal cards.
        int index = UnityEngine.Random.Range(0, _pool.Count);
        PlayCardSound(Box.cards[index], 1);
        Card card = _pool.ElementAt(index);
        _pool.RemoveAt(index);
        return card;
    }

    /// <summary>
    /// Deal a batch of cards
    /// </summary>
    /// <param name="table">The game Table which to deal to</param>
    public void Deal(Table table)
    {
        var dealer = table.GetDealer();
        int cardsDealt = 0;
        PlayCardSound(Audio.sources[0], 4);
        while (!IsDeckEmpty() && cardsDealt < table.Players.Length * table.startingCardCount)
        {
            int playerIndex = (cardsDealt + dealer + 1) % table.Players.Length;
            Card card = GetRandomCard();
            table.Players[playerIndex].Hand.Add(card);
            Cards[FindCardIndex(card)].inPlay = true;
            _pool.Remove(card);
            cardsDealt++;
        }
    }

    /// <summary>
    /// Check how many cards remain in <see cref="_pool"/>
    /// </summary>
    /// <returns>Returns <c>True</c> if no cards remain in <see cref="_pool"/></returns>
    public bool IsDeckEmpty()
    {
        return _pool.Count <= 0;
    }

    /// <summary>
    /// Check if there is enough remaining number of cards in <see cref="_pool"/> <br/>
    /// without having to reshuffle cards back into <see cref="_pool"/>
    /// </summary>
    /// <param name="cardsToDeal">Number of cards that need to be dealt</param>
    /// <returns>Returns <c>True</c> if not enough cards remain in <see cref="_pool"/></returns>
    public bool NotEnoughCards(int cardsToDeal)
    {
        return _pool.Count < cardsToDeal;
    }

    public void PlayCardSound(GameObject card, int srcNum)
    {
        Audio?.Play(card.GetComponent<AudioSource>(), Audio.audios[srcNum]);
    }

    public void PlayCardSound(AudioSource src, int srcNum)
    {
        Audio?.Play(src, Audio.audios[srcNum]);
    }
}
