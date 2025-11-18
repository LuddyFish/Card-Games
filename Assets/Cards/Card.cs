/// <summary>
/// Class <see cref="Card"/> is responsible for the attributes of indiviual cards
/// </summary>
public class Card
{
    public int Suit { get; private set; }
    public int Rank { get; private set; }

    /// <summary>
    /// Check if the card is currently being used
    /// </summary>
    public bool inPlay = false;
    /// <summary>
    /// Is card faced up?
    /// </summary>
    public bool faceUp = false;

    public Card(int Suit, int Rank)
    {
        this.Suit = Suit;
        this.Rank = Rank;
    }

    public enum Suits
    {
        Spade = 0,
        Heart = 1,
        Diamond = 2,
        Club = 3,
    }

    public enum Ranks
    {
        ace = 1,
        two = 2,
        three = 3,
        four = 4,
        five = 5,
        six = 6,
        seven = 7,
        eight = 8,
        nine = 9,
        ten = 10,
        jack = 11,
        queen = 12,
        king = 13,
    }

    /// <summary>
    /// Takes the card's <see cref="Rank"/> and converts it to a string
    /// </summary>
    /// <returns>Name of the card's <see cref="Rank"/></returns>
    public string ConvertRankToString()
    {
        return Rank switch
        {
            (int)Ranks.two => "02",
            (int)Ranks.three => "03",
            (int)Ranks.four => "04",
            (int)Ranks.five => "05",
            (int)Ranks.six => "06",
            (int)Ranks.seven => "07",
            (int)Ranks.eight => "08",
            (int)Ranks.nine => "09",
            (int)Ranks.ten => "10",
            (int)Ranks.jack => "J",
            (int)Ranks.queen => "Q",
            (int)Ranks.king => "K",
            (int)Ranks.ace => "A",
            _ => "JOKER"
        };
    }

    /// <summary>
    /// Takes the card's <see cref="Suit"/> and converts it to a string
    /// </summary>
    /// <returns>Name of the card's <see cref="Suit"/></returns>
    public string ConvertSuitToString()
    {
        return Suit switch
        {
            (int)Suits.Diamond => "Diamond",
            (int)Suits.Club => "Club",
            (int)Suits.Spade => "Spade",
            _ => "Heart"
        };
    }

    /// <summary>
    /// Gets the name of the card
    /// </summary>
    /// <returns>
    /// <c>Suit</c>-<c>Rank</c> as a string
    /// </returns>
    public string GetName()
    {
        string suit = ConvertSuitToString();
        string rank = ConvertRankToString();
        if (rank == "JOKER")
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
            _ => 10
        };
    }
}
