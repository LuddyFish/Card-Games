using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PlayerData
{
    public int id;

    public string name;
    public List<int> cardIds;
    public bool isMyTurn;
    public bool isDealer;

    public PlayerData() { }

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
