[System.Serializable]
public class CardData
{
    public readonly int id;

    public readonly int suit;
    public readonly int rank;
    public readonly bool inPlay;
    public readonly bool faceUp;

    public CardData(Card card)
    {
        id = card.id;
        suit = card.Suit;
        rank = card.Rank;
        inPlay = card.inPlay;
        faceUp = card.faceUp;
    }
}
