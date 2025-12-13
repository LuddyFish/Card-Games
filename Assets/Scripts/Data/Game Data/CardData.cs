[System.Serializable]
public class CardData
{
    public int id;

    public int suit;
    public int rank;
    public bool inPlay;
    public bool faceUp;

    public CardData() { }

    public CardData(Card card)
    {
        id = card.id;
        suit = card.Suit;
        rank = card.Rank;
        inPlay = card.inPlay;
        faceUp = card.faceUp;
    }
}
