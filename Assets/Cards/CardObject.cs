using UnityEngine;

public class CardObject : MonoBehaviour, IDataPersistence<GameData>
{
    SpriteRenderer _rend;

    public Card card;
    public Sprite front, back;

    public bool inHand = false;
    public bool discarded = false;

    void Start()
    {
        _rend = GetComponent<SpriteRenderer>();
    }

    public void LoadData(GameData data)
    {
        foreach (var card in data.cards)
            if (this.card.CompareCard(card.id))
                card.TransferData(this.card);
    }

    public void SaveData(ref GameData data)
    {
        int index = GetCardInDeck();
        if (index == -1)
        {
            Debug.LogError($"Could not find card: \"{name}\" id in Deck");
            return;
        }

        data.cards[index] = new(card);
    }

    void Update()
    {
        _rend.sprite = card.faceUp ? front : back;
    }

    /// <summary>
    /// Finds it's position index in <see cref="Deck.Cards"/>
    /// </summary>
    /// <returns>Returns it's index position or <c>-1</c> if it can't</returns>
    public int GetCardInDeck()
    {
        var deck = BlackjackGameManager.Instance.DeckHandler;
        for (int i = 0; i < deck.Cards.Length; i++)
            if (card.CompareCard(deck.Cards[i]))
                return i;
        return -1;
    }
    /// <summary>
    /// Activates/Deactivates the <c>gameObject</c> depending on if it is <c>inPlay</c>
    /// </summary>
    public void CheckStatus()
    {
        gameObject.SetActive(card.inPlay);
    }

    /// <summary>
    /// Initial card set check
    /// </summary>
    public void CheckCard()
    {
        gameObject.name = card.GetName();
        CheckStatus();
    }

    /// <summary>
    /// Flip to front
    /// </summary>
    public void Reveal()
    {
        card.faceUp = true;
        CheckStatus();
    }

    /// <summary>
    /// Flip to back
    /// </summary>
    public void Hide()
    {
        card.faceUp = false;
        CheckStatus();
    }

    /// <summary>
    /// Flip card based on current status
    /// </summary>
    public void Flip()
    {
        if (card.faceUp)
            Hide();
        else
            Reveal();
    }
}
