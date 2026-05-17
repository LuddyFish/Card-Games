public static class CardUtility
{
    /// <summary>
    /// Takes the card's <see cref="Rank"/> and converts it to a string
    /// </summary>
    /// <returns>Name of the card's <see cref="Rank"/></returns>
    public static string ConvertRankToString(Card.Ranks rank)
    {
        return rank switch
        {
            Card.Ranks.two => "02",
            Card.Ranks.three => "03",
            Card.Ranks.four => "04",
            Card.Ranks.five => "05",
            Card.Ranks.six => "06",
            Card.Ranks.seven => "07",
            Card.Ranks.eight => "08",
            Card.Ranks.nine => "09",
            Card.Ranks.ten => "10",
            Card.Ranks.jack => "J",
            Card.Ranks.queen => "Q",
            Card.Ranks.king => "K",
            Card.Ranks.ace => "A",
            _ => "JOKER"
        };
    }

    /// <summary>
    /// Takes the card's <see cref="Suit"/> and converts it to a string
    /// </summary>
    /// <returns>Name of the card's <see cref="Suit"/></returns>
    public static string ConvertSuitToString(Card.Suits suit)
    {
        return suit switch
        {
            Card.Suits.Diamond => "Diamond",
            Card.Suits.Club => "Club",
            Card.Suits.Spade => "Spade",
            Card.Suits.Heart => "Heart",
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
        string suit = ConvertSuitToString((Card.Suits)card.Suit);
        string rank = ConvertRankToString((Card.Ranks)card.Rank);
        if (rank == "JOKER" || suit == "Joker")
        {
            return rank;
        }
        return $"{suit}-{rank}";
    }



    /// <summary>
    /// The value of cards in blackjack
    /// </summary>
    /// <param name="card"></param>
    /// <returns>Returns the value of the given card</returns>
    public static int BlackjackValue(Card.Ranks card)
    {
        return card switch
        {
            Card.Ranks.ace => 1,
            Card.Ranks.two => 2,
            Card.Ranks.three => 3,
            Card.Ranks.four => 4,
            Card.Ranks.five => 5,
            Card.Ranks.six => 6,
            Card.Ranks.seven => 7,
            Card.Ranks.eight => 8,
            Card.Ranks.nine => 9,
            Card.Ranks.joker => 0,
            _ => 10
        };
    }
}
