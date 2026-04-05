using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cardbox : MonoBehaviour
{
    public static Cardbox Instance { get; private set; }

    [SerializeField] private CardGameContext _gameContext;

    public GameObject cardPrefab;
    public CardDeckSet cardSet;
    public JokerDefinition jokers;

    [HideInInspector] public List<GameObject> cards = new();
    [HideInInspector] public List<GameObject> jokerCards = new();
    public bool isHighContrastMode = false;

    [Space(10)]
    public Vector2 poolLocation;
    public Vector2 discardLocation;
    [SerializeField] private float discardTime = 0.5f;

    public Action OnDealAnimationCompletion;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        _gameContext.ActiveGame.DeckHandler.OnCardDealt += AnimateDeal;
    }

    public void Init()
    {
        for (int i = 0; i < cardSet.cards.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform);
            var obj = card.GetComponent<CardObject>();
            obj.card = _gameContext.Deck.Cards[i];
            _gameContext.CardMap.Add(obj.card, obj);

            SetCardContrast(obj, cardSet.cards[i]);
            SetCard(obj);
            ReturnCard(card.transform);
            cards.Add(card);
        }

        _gameContext.ActiveGame.OnShuffle += ReturnCardsToDeck;
        CardAudio.Instance?.SetCardSRCs();
    }

    public void InitJokers(int players, int startingCount) 
    {
        for (int i = 0; i < startingCount * players; i++)
        {
            CreateJokerCard();
        }

        CardAudio.Instance?.SetJokerCardSRCs();
    }

    public void CreateJokerCard()
    {
        GameObject card = Instantiate(cardPrefab, transform);
        var obj = card.GetComponent<CardObject>();
        obj.card = new Card(jokers.suit, jokers.rank);
        _gameContext.CardMap.Add(obj.card, obj);

        obj.front = jokers.faces[UnityEngine.Random.Range(0, jokers.faces.Length)];
        obj.back = isHighContrastMode ? cardSet.highContrast : cardSet.lowContrast;
        SetCard(obj);
        ReturnCard(obj.transform);
        jokerCards.Add(card);
    }

    /// <summary>
    /// Set the correct card internal properties
    /// </summary>
    /// <param name="card">The <c>CardObject</c> that acts as its memory</param>
    private void SetCard(CardObject card)
    {
        card.card.inPlay = true;
        card.CheckCard();
    }

    /// <summary>
    /// Set the correct sprite on the card
    /// </summary>
    /// <param name="card">The <c>CardObject</c> that acts as its memory</param>
    /// <param name="value">The card template "rules"</param>
    public void SetCardContrast(CardObject card, CardDefinition value)
    {
        card.front = isHighContrastMode ? value.highContrast : value.lowContrast;
        card.back = isHighContrastMode ? cardSet.highContrast : cardSet.lowContrast;
    }

    public void SetJokerContrast(CardObject card)
    {
        card.back = isHighContrastMode ? cardSet.highContrast : cardSet.lowContrast;
    }

    private void GiftCard(PlayerObject player, CardObject card)
    {
        card.inHand = true;
        player.cards.Add(card);

        var layout = player.transform.GetChild(0).GetComponent<HandLayout>();
        card.transform.SetParent(layout.transform);
        layout.ReceiveCard(card.transform, player.cards.Count - 1, player.collectTime);
        StartCoroutine(InvokeAnimationSend(player.collectTime));
        card.GetComponent<SpriteRenderer>().sortingOrder = player.cards.Count - 1;
    }

    private void GiftJoker(PlayerObject player, CardObject card)
    {
        card.inHand = true;
        player.jokers.Add(card);

        var layout = player.transform.GetChild(1).GetComponent<HandLayout>();
        card.transform.SetParent(layout.transform);
        layout.ReceiveCard(card.transform, player.jokers.Count - 1, player.collectTime);
        StartCoroutine(InvokeAnimationSend(player.collectTime));
        card.gameObject.SetActive(true);
        card.Reveal();
        card.GetComponent<SpriteRenderer>().sortingOrder = player.jokers.Count - 1;
    }

    private void AnimateDeal(Player player, Card card)
    {
        if (!_gameContext.PlayerMap.TryGetValue(player, out var playerObj))
        {
            Debug.LogError($"PlayerObject not registered for {player}");
            return;
        }
        if (!_gameContext.CardMap.TryGetValue(card, out var cardObj))
        {
            Debug.LogError($"CardObject not registered for {card}");
            return;
        }

        if (card.Suit != (int)Card.Suits.Joker) 
        { 
            GiftCard(playerObj, cardObj); 
        }
        else
        {
            GiftJoker(playerObj, cardObj);
        }
    }

    private IEnumerator InvokeAnimationSend(float time)
    {
        yield return new WaitForSeconds(time);
        OnDealAnimationCompletion?.Invoke();
    }

    public void ReturnCard(Transform card)
    {
        card.SetParent(transform);
        var obj = card.GetComponent<CardObject>();
        obj.inHand = false;
        obj.discarded = false;
        AnimationUtilities.Lerp(card, card.position, poolLocation, discardTime);
    }

    public void ReturnCardsToDeck()
    {
        _gameContext.Deck.PlayCardSound(CardAudio.Instance.sources[0], 4);
        foreach (var card in cards)
        {
            ReturnCard(card.transform);
        }
    }

    public void DiscardCard(Transform card)
    {
        card.SetParent(transform);
        var obj = card.GetComponent<CardObject>();
        obj.inHand = false;
        obj.discarded = true;
        if (obj.card.Suit == (int)Card.Suits.Joker)
        {
            card.gameObject.SetActive(false);
            card.position = discardLocation;
        }
        else
        {
            AnimationUtilities.Lerp(card, card.position, discardLocation, discardTime);
        }
    }

    public int GetCardPosition(GameObject original)
    {
        for (int i = 0; i < cards.Count; i++)
            if (original.name == cards[i].name)
                return i;
        return 0;
    }

    public bool HasAvailableJoker()
    {
        foreach (var joker in jokerCards)
            if (joker.transform.parent == transform)
                return true;

        return false;
    }

    public GameObject GetAvailableJoker()
    {
        if (!HasAvailableJoker())
            CreateJokerCard();

        foreach (var joker in jokerCards)
            if (joker.transform.parent == transform)
                return joker;

        return null;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
