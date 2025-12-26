using UnityEngine;

[CreateAssetMenu(fileName = "GameManager", menuName = "Game Manager")]
public class CardGameContext : ScriptableObject
{
    public Table Table;
    public Deck Deck;

    [HideInInspector] public CardGameManager ActiveGame;

    public void SetGame(CardGameManager game)
    {
        ActiveGame = game;
        Deck = game.DeckHandler;
        Table = game.TableHandler;
    }

    public void ClearGame()
    {
        ActiveGame = null;
        Deck = null;
        Table = null;
    }
}
