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
        // pre-emptive reset
        players.Clear();
        cards.Clear();

        // set table/player variables
        foreach (Player player in table.Players)
            players.Add(new(player));
        playerTurn = table.playerTurn;
        startingCardCount = table.startingCardCount;

        // set deck/card variables
        foreach (Card card in deck.Cards)
            cards.Add(new(card));

        SaveBlackjackData();
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
