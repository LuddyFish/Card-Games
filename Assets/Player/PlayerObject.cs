using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    Transform hand;

    public Player Data { get; private set; }
    [HideInInspector] public List<CardObject> cards;

    [Tooltip("How long it takes for a card to move into hand")]
    public float collectTime = 0.5f;
    private bool handRevealed = false;

    void Start()
    {
        hand = transform.Find("Hand").transform;
        Data = new Player();
        cards = new List<CardObject>();
    }

    void Update()
    {
        if (Data.isMyTurn && !handRevealed)
            RevealHand();
        else if (!Data.isMyTurn)
            handRevealed = false;
    }

    /// <summary>
    /// Retrieve all cards in <see cref="Data"/> and add it to <see cref="hand"/><br/>
    /// Also resets <see cref="hand"/> to avoid mistakes
    /// </summary>
    public void SetHand()
    {
        foreach (var card in Data.Hand)
            foreach (var obj in Cardbox.Instance.cards)
                if (obj.name == card.GetName())
                {
                    obj.transform.SetParent(hand);
                    break;
                }
        SetCards();
    }

    /// <summary>
    /// Set all cards in <see cref="hand"/> to <see cref="cards"/>
    /// </summary>
    private void SetCards()
    {
        cards.Clear(); // Pre-emptive removal to avoid dupliactes

        var layout = hand.GetComponent<HandLayout>();
        for (int i = 0; i < hand.childCount; i++)
        {
            Transform child = hand.GetChild(i);
            cards.Add(child.GetComponent<CardObject>());
            layout.ReceiveCard(child, i, collectTime);
        }
        layout.SetCards();
    }

    /// <summary>
    /// Removes a specified card from hand and gives it to <see cref="Cardbox"/>
    /// </summary>
    /// <param name="physical">The physical <c>GameObject</c> to return to <see cref="Cardbox"/></param>
    private void RemoveFromHand(Transform physical)
    {
        Cardbox.Instance.DiscardCard(physical);
    }

    /// <summary>
    /// Remove all cards from <see cref="Data"/> and <see cref="hand"/>
    /// </summary>
    public void DiscardCards()
    {
        Data.Hand.Clear();
        while (hand.childCount > 0)
            RemoveFromHand(hand.GetChild(0));
        foreach (var card in cards)
            card.Hide();
        cards.Clear();
    }

    /// <summary>
    /// Turn all cards in <see cref="hand"/> face up
    /// </summary>
    public void RevealHand()
    {
        handRevealed = true;
        foreach (var card in cards)
        {
            card.Reveal();
        }
    }

    /// <summary>
    /// Calculates my score
    /// </summary>
    /// <returns>The sum value of all my cards</returns>
    public int GetScore()
    {
        return BlackjackGameManager.Instance.GetPlayerScore(this);
    }
}
