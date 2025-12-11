using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    // --- Table Data ---
    public List<PlayerData> players;
    public int playerTurn;
    public int startingCardCount;

    // --- Deck Data ---
    public List<CardData> cards;

    // --- Blackjack Data ---
    public struct BlackjackScores
    {
        public int score;
        public int wins;
    }
    public BlackjackScores[] blackjackScores;

    public GameData()
    {
        players = new();
        cards = new();

        foreach (Player player in Table.Players)
            players.Add(new(player));
        playerTurn = Table.playerTurn;
        startingCardCount = Table.startingCardCount;

        foreach (Card card in Deck.Pool)
            cards.Add(new(card));

        if (BlackjackGameManager.Instance != null)
        {
            var scores = BlackjackGameManager.Instance.PlayerScores;
            blackjackScores = new BlackjackScores[scores.Count];
            for (int i = 0; i < blackjackScores.Length; i++)
            {
                blackjackScores[i].score = scores[i].GetScore();
                blackjackScores[i].wins = scores[i].GetWins();
            }
        }
    }
}
