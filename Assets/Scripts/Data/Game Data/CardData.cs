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
        id = card.Id;
        suit = card.Suit;
        rank = card.Rank;
        inPlay = card.inPlay;
        faceUp = card.faceUp;
    }

    /// <summary>
    /// Transfer stored data into <paramref name="card"/>
    /// </summary>
    /// <param name="card"></param>
    public void TransferData(Card card)
    {
        card.Restore(suit, rank);
        card.inPlay = inPlay;
        card.faceUp = faceUp;
    }
}
