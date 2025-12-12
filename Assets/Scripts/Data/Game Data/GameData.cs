using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    // --- Table Data ---
    public List<PlayerData> Players { get; private set; }
    public int playerTurn { get; private set; }
    public int startingCardCount { get; private set; }

    // --- Deck Data ---
    public List<CardData> Cards { get; private set; }

    // --- Blackjack Data ---
    public struct BlackjackScores
    {
        public int score;
        public int wins;
    }
    public BlackjackScores[] blackjackScores { get; private set; }

    public GameData(Player[] Players, Card[] Cards)
    {
        this.Players = new();
        this.Cards = new();

        SaveTableAndDeckData(Players, Cards);

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
        if (this.Players.Count > 0) this.Players.Clear();
        if (this.Cards.Count > 0) this.Cards.Clear();

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
        blackjackScores = null;
        if (BlackjackGameManager.Instance == null) return;

        var scores = BlackjackGameManager.Instance.PlayerScores;
        blackjackScores = new BlackjackScores[scores.Count];
        for (int i = 0; i < blackjackScores.Length; i++)
        {
            blackjackScores[i].score = scores[i].GetScore();
            blackjackScores[i].wins = scores[i].GetWins();
        }
    }
}
