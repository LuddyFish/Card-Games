using System.Collections.Generic;

/// <summary>
/// Class <see cref="Player"/> holds data about the player in the game
/// </summary>
public class Player
{
    public int id { get; private set; }
    public static int nextId = 0;

    public string Name { get; set; }
    public List<Card> Hand { get; private set; }

    public bool isMyTurn = false;
    public bool isDealer = false;

    public Player(string name, int? id = null)
    {
        this.id = id ?? nextId;
        if (id is null)
            nextId++;
        else if (id.Value >= nextId)
            nextId = id.Value + 1;

        Name = name;
        Hand = new List<Card>();
    }

    /// <summary>
    /// Determines if this Player has the same id as <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns>Returns true if both players share the same <see cref="id"/></returns>
    public bool ComparePlayer(Player other)
    {
        return this.id == other.id;
    }

    /// <summary>
    /// Determines if this Player has the same id as <paramref name="other"/>
    /// </summary>
    /// <param name="other">ID of other player</param>
    /// <returns>Returns true if both players share the same <see cref="id"/></returns>
    public bool ComparePlayer(int id)
    {
        return this.id == id;
    }

    /// <summary>
    /// Determines if this Player has the same name as <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns>Returns true if both players share the same <see cref="Name"/></returns>
    public bool CompareName(Player other)
    {
        return this.Name == other.Name;
    }
}
