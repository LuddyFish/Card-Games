/// <summary>
/// Class <see cref="Table"/> is responsible for retaining Player data during the game
/// </summary>
public class Table
{
    /// <summary>
    /// Players of the game sorted in a specified rotation
    /// </summary>
    public Player[] Players { get; private set; }
    /// <summary>
    /// Who's turn it is
    /// </summary>
    public int playerTurn = 0;

    /// <summary>
    /// Number of cards in starting hand
    /// </summary>
    public int startingCardCount = 5;

    public Table(Player[] players, int? playerTurn = null, int? startingCardCount = null)
    {
        Players = players;
        this.playerTurn = playerTurn ?? this.playerTurn;
        this.startingCardCount = startingCardCount ?? this.startingCardCount;
    }

    /// <summary>
    /// Creates a new table with the given players
    /// </summary>
    /// <param name="players">All the players entering the game</param>
    public void NewTable(Player[] players)
    {
        Players = new Player[players.Length];
        for (int i = 0; i < players.Length; i++)
            SetPlayer(i, players[i]);
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
    /// Sets the player position in the rotation
    /// </summary>
    /// <param name="position"></param>
    /// <param name="player"></param>
    public void SetPlayer(int position, Player player)
    {
        Players[position] = player;
    }

    /// <summary>
    /// Move to the next player in the rotation
    /// </summary>
    public void NextPlayerTurn()
    {
        RestPlayer(Players[playerTurn]);
        playerTurn = (playerTurn + 1) % Players.Length;
        WakePlayer(Players[playerTurn]);
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
