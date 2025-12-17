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

    /// <summary>
    /// Transfer stored data into <paramref name="player"/>
    /// </summary>
    /// <param name="player"></param>
    public void TransferData(Player player, Dictionary<int, Card> cardById)
    {
        player.Name = name;

        player.Hand.Clear();
        foreach (var id in cardIds)
            if (cardById.TryGetValue(id, out var card))
                player.Hand.Add(card);

        player.isMyTurn = isMyTurn;
        player.isDealer = isDealer;
    }
}
