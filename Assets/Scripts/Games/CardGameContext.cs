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
}
