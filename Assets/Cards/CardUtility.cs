public static class CardUtility
{
    /// <summary>
    /// Takes the card's <see cref="Rank"/> and converts it to a string
    /// </summary>
    /// <returns>Name of the card's <see cref="Rank"/></returns>
    public static string ConvertRankToString(Ranks rank)
    {
        return rank switch
        {
            Ranks.two => "02",
            Ranks.three => "03",
            Ranks.four => "04",
            Ranks.five => "05",
            Ranks.six => "06",
            Ranks.seven => "07",
            Ranks.eight => "08",
            Ranks.nine => "09",
            Ranks.ten => "10",
            Ranks.jack => "J",
            Ranks.queen => "Q",
            Ranks.king => "K",
            Ranks.ace => "A",
            _ => "JOKER"
        };
    }

    /// <summary>
    /// Takes the card's <see cref="Suit"/> and converts it to a string
    /// </summary>
    /// <returns>Name of the card's <see cref="Suit"/></returns>
    public static string ConvertSuitToString(Suits suit)
    {
        return suit switch
        {
            Suits.Diamond => "Diamond",
            Suits.Club => "Club",
            Suits.Spade => "Spade",
            Suits.Heart => "Heart",
            _ => "Joker"
        };
    }

    /// <summary>
    /// Gets the name of the card
    /// </summary>
    /// <returns>
    /// <see cref="Card.Suit"/>-<see cref="Card.Rank"/> as a string
    /// </returns>
    public static string GetName(Card card)
    {
        string suit = ConvertSuitToString((Suits)card.Suit);
        string rank = ConvertRankToString((Ranks)card.Rank);
        if (rank == "JOKER" || suit == "Joker")
        {
            return rank;
        }
        return $"{suit}-{rank}";
    }
}
