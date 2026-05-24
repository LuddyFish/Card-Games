/// <summary>
/// Class <see cref="Card"/> is responsible for the attributes of indiviual cards
/// </summary>
public class Card
{
    private readonly int _id;
    public int Id => _id;
    private static int _nextId = 0;

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

    public Card(int Suit, int Rank, int? id = null)
    {
        this._id = id ?? _nextId;
        if (id is null)
            _nextId++;
        else if (id.Value >= _nextId)
            _nextId = id.Value + 1;

        this.Suit = Suit;
        this.Rank = Rank;
    }

    public override string ToString()
    {
        return $"Name: {CardUtility.GetName(this)}\nID: {Id}\ninPlay: {inPlay}\nFaceUp: {faceUp}";
    }

    /// <summary>
    /// Determines if this card is the same id as <paramref name="other"/>
    /// </summary>
    /// <param name="other">Card to compare to</param>
    /// <returns>Returns true if card has the same <see cref="Card.Id"/></returns>
    public bool CompareCard(Card other)
    {
        return this.Id == other.Id;
    }

    /// <summary>
    /// Determines if this card is the same id as <paramref name="other"/>
    /// </summary>
    /// <param name="Id">ID of the other card</param>
    /// <returns>Returns true if card has the same <see cref="Card.Id"/></returns>
    public bool CompareCard(int Id)
    {
        return this.Id == Id;
    }

    /// <summary>
    /// Restore the original values of the card
    /// </summary>
    /// <param name="suit"></param>
    /// <param name="rank"></param>
    public void Restore(int suit, int rank)
    {
        Suit = suit;
        Rank = rank;
    }
}

public enum Suits
{
    Spade = 0,
    Heart = 1,
    Diamond = 2,
    Club = 3,
    Joker = -1
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
    joker = -1
}
