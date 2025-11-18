using System.Collections.Generic;

/// <summary>
/// Class <see cref="Player"/> holds data about the player in the game
/// </summary>
public class Player
{
    public List<Card> Hand { get; private set; }

    public bool isMyTurn = false;
    public bool isDealer = false;

    public Player()
    {
        Hand = new List<Card>();
    }
}
