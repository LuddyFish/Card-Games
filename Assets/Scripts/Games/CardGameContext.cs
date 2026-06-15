using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameManager", menuName = "Game Manager")]
public class CardGameContext : ScriptableObject
{
    public Table Table;
    public Deck Deck;

    public CardGameManager ActiveGame { get; private set; }

    public Dictionary<Player, PlayerObject> PlayerMap { get; } = new();
    public Dictionary<Card, CardObject> CardMap { get; } = new();
        
    public void SetGame(CardGameManager game)
    {
        ActiveGame = game;
        Deck = game.DeckHandler;
        Table = game.TableHandler;

        PlayerMap.Clear();
        CardMap.Clear();
    }

    public void ClearGame()
    {
        ActiveGame = null;
        Deck = null;
        Table = null;

        PlayerMap.Clear();
        CardMap.Clear();
    }

    /// <summary>
    /// Get the <see cref="PlayerObject"/> attached to <see cref="Player"/>
    /// </summary>
    /// <param name="player">The player variable</param>
    /// <returns></returns>
    public PlayerObject GetPlayerObject(Player player)
    {
        if (PlayerMap.TryGetValue(player, out var obj)) return obj;
        Debug.LogError($"PlayerObject not registered for {player}");
        return null;
    }
    
    /// <summary>
    /// Get the <see cref="CardObject"/> attached to <see cref="Card"/>
    /// </summary>
    /// <param name="card">The player variable</param>
    /// <returns></returns>
    public CardObject GetCardObject(Card card)
    {
        if (CardMap.TryGetValue(card, out var obj)) return obj;
        Debug.LogError($"CardObject not registered for {card}");
        return null;
    }
}
