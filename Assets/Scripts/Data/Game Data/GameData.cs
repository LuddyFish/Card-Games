using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // --- Table Data ---
    public List<PlayerData> Players = new();
    public int playerTurn;
    public int startingCardCount;

    // --- Deck Data ---
    public List<CardData> Cards = new();

    // --- Blackjack Data ---
    public BlackjackScore[] BlackjackScores;

    [System.Serializable]
    public struct BlackjackScore
    {
        public int score;
        public int wins;
    }

    public GameData() { }

    public GameData(Player[] players, Card[] cards)
    {
        SaveTableAndDeckData(players, cards);
        SaveBlackjackData();
    }

    /// <summary>
    /// Save specific data from <see cref="Table"/> and <see cref="Deck"/>.
    /// </summary>
    /// <param name="Players"><see cref="Table.Players"/></param>
    /// <param name="Cards"><see cref="Deck.Cards"/></param>
    public void SaveTableAndDeckData(Player[] Players, Card[] Cards)
    {
        // pre-emptive reset
        this.Players.Clear();
        this.Cards.Clear();

        // set table/player variables
        foreach (Player player in Players)
            this.Players.Add(new(player));
        playerTurn = Table.playerTurn;
        startingCardCount = Table.startingCardCount;

        // set deck/card variables
        foreach (Card card in Cards)
            this.Cards.Add(new(card));
    }

    public void SaveBlackjackData()
    {
        // clear the save if it has already been set
        if (BlackjackGameManager.Instance == null)
        {
            BlackjackScores = null;
            return;
        }

        var scores = BlackjackGameManager.Instance.PlayerScores;
        BlackjackScores = new BlackjackScore[scores.Count];

        for (int i = 0; i < BlackjackScores.Length; i++)
        {
            BlackjackScores[i].score = scores[i].GetScore();
            BlackjackScores[i].wins = scores[i].GetWins();
        }
    }
}
