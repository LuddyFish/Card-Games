public static class BlackjackScorer
{
    /// <summary>
    /// Calculates the Player's total score
    /// </summary>
    /// <param name="player"></param>
    /// <returns>The sum value of all cards</returns>
    public static int GetPlayerScore(PlayerObject player)
    {
        int score = 0;
        bool ace = false;
        foreach (var card in player.cards)
        {
            int value = BlackjackValue((Ranks)card.card.Rank);
            if (value == 1)
                ace = true;
            score += value;
        }
        if (ace && score + 10 <= 21) // Account for the fact that ace is a value of 1 OR 11
            score += 10;
        return score;
    }

    /// <summary>
    /// Select the Player with the highest legal score
    /// </summary>
    /// <returns>The winner</returns>
    public static PlayerObject GetWinner(PlayerObject[] players)
    {
        PlayerObject winner = null;
        int highest = 0;

        foreach (var player in players)
        {
            int score = GetPlayerScore(player);
            if (score <= 21 && score > highest)
            {
                highest = score;
                winner = player;
            }
        }

        return winner;
    }

    /// <summary>
    /// Select the Player with the highest legal score
    /// </summary>
    /// <returns>The winning Player's index position</returns>
    public static int GetWinnerIndex(PlayerObject[] Players)
    {
        int winner = -1;
        int highest = 0;

        for (int i = 0; i < Players.Length; i++)
        {
            int score = GetPlayerScore(Players[i]);
            if (score <= 21 && score > highest)
            {
                highest = score;
                winner = i;
            }
        }

        return winner;
    }

    /// <summary>
    /// Checks the players score if less than 21
    /// </summary>
    /// <param name="player"></param>
    /// <returns>Returns <c>TRUE</c> if the player has less than 21</returns>
    public static bool CanHit(PlayerObject player)
    {
        if (GetPlayerScore(player) >= 21)
            return false;
        else
            return true;
    }

    /// <summary>
    /// The value of cards in blackjack
    /// </summary>
    /// <param name="card"></param>
    /// <returns>Returns the value of the given card</returns>
    public static int BlackjackValue(Ranks card)
    {
        return card switch
        {
            Ranks.ace => 1,
            Ranks.two => 2,
            Ranks.three => 3,
            Ranks.four => 4,
            Ranks.five => 5,
            Ranks.six => 6,
            Ranks.seven => 7,
            Ranks.eight => 8,
            Ranks.nine => 9,
            Ranks.joker => 0,
            _ => 10
        };
    }
}
