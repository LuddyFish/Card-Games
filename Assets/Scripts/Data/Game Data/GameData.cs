using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // --- Table Data ---
    public List<PlayerData> players = new();
    public int playerTurn;
    public int startingCardCount;

    // --- Deck Data ---
    public List<CardData> cards = new();

    // --- Blackjack Data ---
    public BlackjackScore[] blackjackScores;

    [System.Serializable]
    public struct BlackjackScore
    {
        public int score;
        public int wins;
    }

    public GameData() { }

    public GameData(Table table, Deck deck)
    {
        SaveTableData(table);
        SaveDeckData(deck);
    }

    public void SaveTableData(Table table)
    {
        players.Clear();

        foreach (Player player in table.Players)
            players.Add(new(player));
        playerTurn = table.PlayerTurn;
        startingCardCount = table.StartingCardCount;
    }

    public Player[] LoadPlayers(Dictionary<int, Player> playerById)
    {
        List<Player> list = new();

        foreach (var pData in players)
            if (playerById.TryGetValue(pData.id, out var player))
                list.Add(player);

        return list.ToArray();
    }

    public void SaveDeckData(Deck deck)
    {
        cards.Clear();

        foreach (Card card in deck.Cards)
            cards.Add(new(card));
    }

    public Card[] LoadCards(Dictionary<int, Card> cardById)
    {
        List<Card> list = new();

        foreach (var cData in cards)
            if (cardById.TryGetValue(cData.id, out var card))
                list.Add(card);

        return list.ToArray();
    }

    public void SaveBlackjackData(BlackjackGameManager BJGM)
    {
        var scores = BJGM.PlayerScores;
        blackjackScores = new BlackjackScore[scores.Count];

        for (int i = 0; i < blackjackScores.Length; i++)
        {
            blackjackScores[i].score = scores[i].Scores;
            blackjackScores[i].wins = scores[i].Wins;
        }
    }
}
