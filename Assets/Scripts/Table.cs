using System;
using System.Linq;

/// <summary>
/// Class <see cref="Table"/> is responsible for retaining Player data during the game
/// </summary>
public class Table : IDataPersistence<GameData>
{
    /// <summary>
    /// Players of the game sorted in a specified rotation
    /// </summary>
    public Player[] Players { get; private set; }
    /// <summary>
    /// Who's turn it is
    /// </summary>
    public int PlayerTurn { get; private set; }

    /// <summary>
    /// Number of cards in starting hand
    /// </summary>
    public int StartingCardCount { get; private set; }

    public Table(Player[] Players, int PlayerTurn = 0, int StartingCardCount = 5)
    {
        NewTable(Players);

        if (PlayerTurn < 0 || PlayerTurn >= Players.Length)
            throw new ArgumentOutOfRangeException(nameof(PlayerTurn));
        this.PlayerTurn = PlayerTurn;

        if (StartingCardCount < 0)
            throw new ArgumentOutOfRangeException(nameof(StartingCardCount));
        this.StartingCardCount = StartingCardCount;
    }

    /// <summary>
    /// Creates a new table with the given players
    /// </summary>
    /// <param name="players">All the players entering the game</param>
    private void NewTable(Player[] players)
    {
        Players = new Player[players.Length];
        for (int i = 0; i < players.Length; i++)
            Players[i] = players[i];
    }

    public void LoadData(GameData data)
    {
        Players = data.LoadPlayers(Players.ToDictionary(p => p.Id));
        PlayerTurn = data.playerTurn;
        StartingCardCount = data.startingCardCount;
    }

    public void SaveData(ref GameData data)
    {
        data.SaveTableData(this);
    }

    /// <summary>
    /// Returns a Player by index
    /// </summary>
    /// <param name="index">Player number</param>
    /// <returns>Returns the specified Player</returns>
    public Player GetPlayer(int index)
    {
        int playerNumber = index % Players.Length;
        return Players[playerNumber];
    }

    /// <summary>
    /// Move to the next player in the rotation
    /// </summary>
    public void NextPlayerTurn()
    {
        RestPlayer(Players[PlayerTurn]);
        PlayerTurn = (PlayerTurn + 1) % Players.Length;
        WakePlayer(Players[PlayerTurn]);
    }

    public void SetPlayerTurn(int playerNum)
    {
        PlayerTurn = playerNum;
    }

    /// <summary>
    /// Return the dealer's number in rotation
    /// </summary>
    /// <returns>The Player set as the dealer's position number</returns>
    public int GetDealer()
    {
        for (int i = 0; i < Players.Length; i++)
            if (Players[i].isDealer) 
                return i;
        // Set first player in list as dealer if none are dealers
        SetDealer(Players[0]);
        return 0;
    }

    /// <summary>
    /// Set the dealer
    /// </summary>
    /// <param name="player"></param>
    public void SetDealer(Player player)
    {
        player.isDealer = true;
    }

    /// <summary>
    /// Change the dealer to a specified player
    /// </summary>
    /// <param name="previous">The Player currently set dealer</param>
    /// <param name="current">The Player next to be set dealer</param>
    public void SwapDealer(Player previous, Player current)
    {
        previous.isDealer = false;
        SetDealer(current);
    }

    /// <summary>
    /// Change the dealer to the next person in rotation
    /// </summary>
    public void GetNextDealer()
    {
        int currentDealer = GetDealer();
        int nextDealer = currentDealer++ % Players.Length;
        SwapDealer(Players[currentDealer], Players[nextDealer]);
    }

    /// <summary>
    /// Set the Player's turn as active
    /// </summary>
    /// <param name="player"></param>
    public void WakePlayer(Player player)
    {
        player.isMyTurn = true;
    }

    /// <summary>
    /// Set the Player's turn as disabled
    /// </summary>
    /// <param name="player"></param>
    public void RestPlayer(Player player)
    {
        player.isMyTurn = false;
    }
}
