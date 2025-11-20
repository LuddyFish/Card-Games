using System.Collections.Generic;

/// <summary>
/// Class <see cref="Player"/> holds data about the player in the game
/// </summary>
public class Player
{
    public string Name { get; set; }
    public List<Card> Hand { get; private set; }

    public bool isMyTurn = false;
    public bool isDealer = false;

    public Player(string name)
    {
        Name = name;
        Hand = new List<Card>();
    }
}
