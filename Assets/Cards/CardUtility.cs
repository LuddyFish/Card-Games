using System.Collections.Generic;

public static class CardUtility
{
    private static readonly Dictionary<Ranks, string> RankStrings = new()
    {
        { Ranks.two,   "02" },
        { Ranks.three, "03" },
        { Ranks.four,  "04" },
        { Ranks.five,  "05" },
        { Ranks.six,   "06" },
        { Ranks.seven, "07" },
        { Ranks.eight, "08" },
        { Ranks.nine,  "09" },
        { Ranks.ten,   "10" },
        { Ranks.jack,  "J"  },
        { Ranks.queen, "Q"  },
        { Ranks.king,  "K"  },
        { Ranks.ace,   "A"  },
    };

    private static readonly Dictionary<Suits, string> SuitStrings = new()
    {
        { Suits.Diamond, "Diamond" },
        { Suits.Club,    "Club"    },
        { Suits.Spade,   "Spade"   },
        { Suits.Heart,   "Heart"   },
    };

    private static readonly string rankJoker = "JOKER";
    private static readonly string suitJoker = "Joker";

    /// <summary>
    /// Takes the card's <see cref="Rank"/> and converts it to a string
    /// </summary>
    /// <returns>Name of the card's <see cref="Rank"/></returns>
    public static string ConvertRankToString(Ranks rank)
    {
        return RankStrings.GetValueOrDefault(rank, rankJoker);
    }

    /// <summary>
    /// Takes the card's <see cref="Suit"/> and converts it to a string
    /// </summary>
    /// <returns>Name of the card's <see cref="Suit"/></returns>
    public static string ConvertSuitToString(Suits suit)
    {
        return SuitStrings.GetValueOrDefault(suit, suitJoker);
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
        if (rank == rankJoker || suit == suitJoker)
        {
            return rank;
        }
        return $"{suit}-{rank}";
    }
}
