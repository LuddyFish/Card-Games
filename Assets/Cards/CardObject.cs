using UnityEngine;

[RequireComponent(typeof(ClickableSprite))]
public class CardObject : MonoBehaviour, IDataPersistence
{
    [HideInInspector] public CardGameContext context;

    SpriteRenderer _rend;
    ClickableSprite _clickSprite;

    public Card card;
    public Sprite front, back;

    [Space(10)]
    public bool isSelectable = false;
    public bool selected = false;
    public float selectedRaise = 0.5f;

    [Space(10)]
    public bool inHand = false;
    public bool discarded = false;

    void Start()
    {
        _rend = GetComponent<SpriteRenderer>();
        _clickSprite = GetComponent<ClickableSprite>();
        _clickSprite.OnTrueClick += Select;
        InputManager.Instance.OnRightClick += Deselect;

        _clickSprite.enabled = isSelectable;
    }

    public void LoadData(GameData data)
    {
        if (DataPersistenceManager.Instance == null) return;

        foreach (var card in data.cards)
            if (this.card.CompareCard(card.id))
                card.TransferData(this.card);
    }

    public void SaveData(ref GameData data)
    {
        int index = GetCardInDeck();
        if (index <= -1)
        {
            Debug.LogError($"Could not find card: \"{name}\" id in Deck");
            return;
        }

        data.cards[index] = new(card);
    }

    /// <summary>
    /// Finds it's position index in <see cref="Deck.Cards"/>
    /// </summary>
    /// <returns>Returns it's index position or <c>-1</c> if it can't</returns>
    public int GetCardInDeck()
    {
        var deck = context.Deck;
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
        _rend.sprite = card.faceUp ? front : back;
        _clickSprite.enabled = isSelectable;
    }

    /// <summary>
    /// Initial card set check
    /// </summary>
    public void CheckCard()
    {
        gameObject.name = CardUtility.GetName(card);
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
        _rend.sprite = back;
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

    private void ToggleSelect()
    {
        transform.position += new Vector3(0, selected ? selectedRaise : -selectedRaise);
    }

    private void Select()
    {
        selected = true;
        ToggleSelect();
    }

    private void Deselect()
    {
        selected = false;
        ToggleSelect();
    }
}
