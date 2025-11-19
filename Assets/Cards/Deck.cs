using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class <see cref="Deck"/> is responsible for maintaining the cards in the deck
/// </summary>
public static class Deck
{
    /// <summary>
    /// Whole deck of cards. This should not be modified!
    /// </summary>
    public static Card[] Cards { get; private set; }
    /// <summary>
    /// Deck of cards that can be selected from
    /// </summary>
    public static List<Card> Pool { get; private set; }
    // How many suits and ranks
    private static int suitCount = 4;
    private static int rankCount = 13;

    /// <summary>
    /// Box where the physical <c>GameObject</c>s are stored
    /// </summary>
    private static Cardbox Box => Cardbox.Instance;

    /// <summary>
    /// Initialise the deck
    /// </summary>
    public static void InitDeck()
    {
        suitCount = Enum.GetNames(typeof(Card.Suits)).Length;
        rankCount = Enum.GetNames(typeof(Card.Ranks)).Length;
        Cards = new Card[suitCount * rankCount];
        for (int i = 0; i < suitCount; i++)
            for (int j = 1; j <= rankCount; j++)
                Cards[i * rankCount + j - 1] = new Card(i, j);
        Pool = new List<Card>();
        Box.Init();
    }

    /// <summary>
    /// Reset the cards in the pool to choose from.
    /// </summary>
    public static void NewDeck()
    {
        Pool.Clear();
        foreach (Card card in Cards)
            Pool.Add(card);
    }

    /// <summary>
    /// Reset the cards in the pool to choose from but leaves all cards 
    /// </summary>
    public static void NewSoftDeck()
    {
        if (Box != null)
        {
            Pool.Clear();
            for (int i = 0; i < Cards.Length; i++)
            {
                var card = Box.cards[i].GetComponent<CardObject>().card;
                Pool.Add(card);
                if (!card.faceUp) Box.ReturnCard(Box.cards[i].transform);
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
    private static int FindCardIndex(Card card)
    {
        for (int i = 0; i < Cards.Length; i++)
            if (Cards[i] == card)
                return i;
        return 0;
    }

    /// <summary>
    /// Get a random card from <see cref="Pool"/>
    /// </summary>
    /// <returns>Returns a random card in <see cref="Pool"/></returns>
    private static Card GetRandomCard()
    {
        int index = UnityEngine.Random.Range(0, Pool.Count);
        return Pool.ElementAt(index);
    }

    /// <summary>
    /// Get a random card in <see cref="Pool"/>
    /// </summary>
    /// <returns>A card</returns>
    public static Card DealRandomCard()
    {
        if (IsDeckEmpty()) NewSoftDeck(); // Ensures that can deal cards.
        int index = UnityEngine.Random.Range(0, Pool.Count);
        Card card = Pool.ElementAt(index);
        Pool.RemoveAt(index);
        return card;
    }

    /// <summary>
    /// Deal a batch of cards
    /// </summary>
    public static void Deal()
    {
        var dealer = Table.GetDealer();
        int cardsDealt = 0;
        while (!IsDeckEmpty() && cardsDealt < Table.Players.Length * Table.startingCardCount)
        {
            int playerIndex = (cardsDealt + dealer + 1) % Table.Players.Length;
            Card card = GetRandomCard();
            Table.Players[playerIndex].Hand.Add(card);
            Cards[FindCardIndex(card)].inPlay = true;
            Pool.Remove(card);
            cardsDealt++;
        }
    }

    /// <summary>
    /// Check how many cards remain in <see cref="Pool"/>
    /// </summary>
    /// <returns>Returns <c>True</c> if no cards remain in <see cref="Pool"/></returns>
    public static bool IsDeckEmpty()
    {
        return Pool.Count <= 0;
    }

    /// <summary>
    /// Check if there is enough remaining number of cards in <see cref="Pool"/> <br/>
    /// without having to reshuffle cards back into <see cref="Pool"/>
    /// </summary>
    /// <returns>Returns <c>True</c> if not enough cards remain in <see cref="Pool"/></returns>
    public static bool NotEnoughCards()
    {
        return Pool.Count < Table.Players.Length * Table.startingCardCount;
    }
}
