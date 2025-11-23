using UnityEngine;

public class CardObject : MonoBehaviour
{
    SpriteRenderer rend;

    public Card card;
    public Sprite front, back;

    public bool inHand = false;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        rend.sprite = card.faceUp ? front : back;
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
