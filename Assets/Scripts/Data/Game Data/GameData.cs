using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    // --- Player Settings 
    public float volume;
    public string backgroundId;
    public bool highContrast;

    // --- Player Stats ---
    public int blackjackWins;
    public int blackjackGames;

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
        volume = 1.0f;
        backgroundId = "Classic";
        highContrast = false;

        blackjackGames = 0;
        blackjackWins = 0;
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

    public float GetBlackjackWinPercentage()
    {
        return (float)blackjackWins / blackjackGames;
    }
}
