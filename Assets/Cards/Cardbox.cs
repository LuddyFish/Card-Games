using System.Collections.Generic;
using UnityEngine;

public class Cardbox : MonoBehaviour
{
    public static Cardbox Instance;

    public GameObject cardPrefab;

    public Sprite back;
    public Sprite[] face = new Sprite[52];

    [HideInInspector] public List<GameObject> cards = new();

    [Space(10)]
    public Vector2 discardLocation;
    [SerializeField] private float discardTime = 0.5f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void Init()
    {
        for (int i = 0; i < face.Length; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform);
            var obj = card.GetComponent<CardObject>();
            obj.card = Deck.Cards[i];
            SetCardSprite(obj);
            ReturnCard(card.transform);
            cards.Add(card);
        }
        if (BlackjackGameManager.Instance != null) BlackjackGameManager.Instance.onShuffle += ReturnCardsToDeck;
        CardAudio.Instance?.SetCardSRCs();
    }

    /// <summary>
    /// Set the correct sprite on the card
    /// </summary>
    /// <param name="card">The <c>CardObject</c> that acts as its memory</param>
    private void SetCardSprite(CardObject card)
    {
        // Set the front face
        foreach (var sprite in face)
        {
            if (sprite.name.Contains(card.card.ConvertRankToString()) && 
                sprite.name.Contains(card.card.ConvertSuitToString().ToLower())
            )
            {
                card.front = sprite;
                card.card.inPlay = true;
                card.CheckCard();
                break;
            }
        }
        // Set the back cover
        card.back = back;
    }

    public void ReturnCard(Transform card)
    {
        card.SetParent(transform);
        AnimationUtilities.Lerp(card, card.position, transform.position, discardTime);
    }

    public void ReturnCardsToDeck()
    {
        Deck.PlayCardSound(CardAudio.Instance.sources[0], 4);
        foreach (var card in cards)
        {
            ReturnCard(card.transform);
        }
    }

    public void DiscardCard(Transform card)
    {
        card.SetParent(transform);
        card.GetComponent<CardObject>().inHand = false;
        AnimationUtilities.Lerp(card, card.position, discardLocation, discardTime);
    }

    public int GetCardPosition(GameObject original)
    {
        for (int i = 0; i < cards.Count; i++)
            if (original.name == cards[i].name)
                return i;
        return 0;
    }
}
