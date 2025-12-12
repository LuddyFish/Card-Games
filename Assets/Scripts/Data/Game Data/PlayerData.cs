using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PlayerData
{
    public readonly int id;

    public readonly string name;
    public readonly List<int> cardIds;
    public readonly bool isMyTurn;
    public readonly bool isDealer;

    public PlayerData(Player player)
    {
        cardIds = new();

        id = player.id;
        name = player.Name;
        cardIds = player.Hand.Select(c => c.id).ToList();

        isMyTurn = player.isMyTurn;
        isDealer = player.isDealer;
    }
}
