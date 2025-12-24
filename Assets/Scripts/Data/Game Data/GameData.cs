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

    public GameData()
    {
        SaveTableAndDeckData();
        SaveBlackjackData();
    }

    /// <summary>
    /// Save specific data from <see cref="Table"/> and <see cref="Deck"/>.
    /// </summary>
    public void SaveTableAndDeckData()
    {
        // pre-emptive reset
        players.Clear();
        cards.Clear();

        // set table/player variables
        foreach (Player player in Table.Players)
            players.Add(new(player));
        playerTurn = Table.playerTurn;
        startingCardCount = Table.startingCardCount;

        // set deck/card variables
        foreach (Card card in Deck.Cards)
            cards.Add(new(card));
    }

    public void SaveBlackjackData()
    {
        // clear the save if it has already been set
        if (BlackjackGameManager.Instance == null)
        {
            blackjackScores = null;
            return;
        }

        var scores = BlackjackGameManager.Instance.PlayerScores;
        blackjackScores = new BlackjackScore[scores.Count];

        for (int i = 0; i < blackjackScores.Length; i++)
        {
            blackjackScores[i].score = scores[i].Scores;
            blackjackScores[i].wins = scores[i].Wins;
        }
    }
}
