using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cardbox : MonoBehaviour
{
    public static Cardbox Instance { get; private set; }

    [SerializeField] private CardGameContext _gameContext;
    CardAudio _cardAudio => CardAudio.Instance;

    public GameObject cardPrefab;
    public CardDeckSet cardSet;
    public JokerDefinition jokers;

    private readonly Dictionary<GameObject, int> _cardIndexMap = new();

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
            obj.context = _gameContext;
            _gameContext.CardMap.Add(obj.card, obj);

            SetCardContrast(obj, cardSet.cards[i]);
            SetCard(obj);
            ReturnCard(card.transform);
            cards.Add(card);
            _cardIndexMap[card] = i;

            _cardAudio?.RegisterCardSRC(card.GetComponent<AudioSource>());
        }

        _gameContext.Deck.OnDeckShuffled += ReturnCardsToDeck;
        _gameContext.Deck.OnDeckSoftShuffled += RecallInactiveCards;
    }

    public void InitJokers(int players, int startingCount) 
    {
        for (int i = 0; i < startingCount * players; i++) 
        {
            CreateJokerCard();
        }
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

        _cardAudio?.RegisterCardSRC(card.GetComponent<AudioSource>());
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

    private void GiftCard(PlayerObject player, CardObject card, bool isJoker)
    {
        card.inHand = true;

        var targetHand = isJoker ? player.jokerHand : player.hand;
        var targetList = isJoker ? player.jokers : player.cards;
        targetList.Add(card);

        var layout = targetHand.GetComponent<HandLayout>();
        card.transform.SetParent(targetHand);
        layout.ReceiveCard(card.transform, targetList.Count - 1, player.collectTime);
        
        StartCoroutine(InvokeAnimationSend(player.collectTime));
        if (isJoker)
        {
            card.gameObject.SetActive(true);
            card.Reveal();
        }
        card.GetComponent<SpriteRenderer>().sortingOrder = targetList.Count - 1;
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

        _cardAudio?.PlayCardDeal(cardObj.GetComponent<AudioSource>());
        GiftCard(playerObj, cardObj, card.Suit == (int)Suits.Joker); 
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
        // _cardAudio?.PlayCardShuffle(_cardAudio.sources[0]);
        foreach (var card in cards)
        {
            ReturnCard(card.transform);
        }
    }

    public void RecallInactiveCards()
    {
        // _cardAudio?.PlayCardShuffle(_cardAudio.sources[0]);
        foreach (var card in cards)
        {
            var c = card.GetComponent<CardObject>();
            if (!c.inHand)
            {
                ReturnCard(card.transform);
            }
        }
    }

    public void DiscardCard(Transform card)
    {
        card.SetParent(transform);
        var obj = card.GetComponent<CardObject>();
        obj.inHand = false;
        obj.discarded = true;
        if (obj.card.Suit == (int)Suits.Joker)
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
        return _cardIndexMap.TryGetValue(original, out int index) ? index : 0;
    }

    public bool TryGetAvaibleJoker(out GameObject joker)
    {
        foreach (var j in jokerCards)
        {
            if (j.transform.parent == transform)
            {
                joker = j;
                return true;
            }
        }

        CreateJokerCard();
        joker = jokerCards[^1];
        return true;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
