public class PlayerGameStats
{
    public int blackjackWins;
    public int blackjackGames;

    public PlayerGameStats()
    {
        blackjackWins = 0;
        blackjackGames = 0;
    }

    public float GetBlackjackWinPercentage()
    {
        return (float)blackjackWins / blackjackGames;
    }
}
