using System;
using System.Collections.Generic;

/// <summary>
/// Class <see cref="Player"/> holds data about the player in the game
/// </summary>
public class Player
{
    private readonly int _id;
    public int Id => _id;
    private static int _nextId = 0;

    public string name;
    public List<Card> Hand { get; private set; }
    public List<Card> Jokers { get; private set; }

    public bool isDealer = false;

    public Action OnTurnEnable;
    public Action OnTurnDisable;

    public Player(string name, int? id = null)
    {
        _id = id ?? _nextId;
        if (id is null)
            _nextId++;
        else if (id.Value >= _nextId)
            _nextId = id.Value + 1;

        this.name = name;
        Hand = new List<Card>();
        Jokers = new List<Card>();
    }

    public override string ToString()
    {
        string cards = string.Empty;
        for (int i = 0; i < Hand.Count; i++)
            cards += $"\tCard {i + 1}:\n{Hand[i]}\n";
        for (int i = 0; i < Jokers.Count; i++)
            cards += $"\tJoker {i + 1}:\n{Jokers[i]}\n";

        return $"Name: {name}\nID: {Id}\nCards:\n{cards}\nIs Dealer: {isDealer}";
    }

    /// <summary>
    /// Determines if this Player has the same id as <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns>Returns true if both players share the same <see cref="Id"/></returns>
    public bool ComparePlayer(Player other)
    {
        return this.Id == other.Id;
    }

    /// <summary>
    /// Determines if this Player has the same id as <paramref name="other"/>
    /// </summary>
    /// <param name="other">ID of other player</param>
    /// <returns>Returns true if both players share the same <see cref="Id"/></returns>
    public bool ComparePlayer(int Id)
    {
        return this.Id == Id;
    }

    /// <summary>
    /// Determines if this Player has the same name as <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns>Returns true if both players share the same <see cref="name"/></returns>
    public bool CompareName(Player other)
    {
        return this.name == other.name;
    }
}
