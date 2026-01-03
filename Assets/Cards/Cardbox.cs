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

    [HideInInspector] public List<GameObject> cards = new();
    [SerializeField] private bool _isHighContrastMode = false;

    [Space(10)]
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
    private void SetCardContrast(CardObject card, CardDefinition value)
    {
        card.front = _isHighContrastMode ? value.highContrast : value.lowContrast;
        card.back = _isHighContrastMode ? cardSet.highContrast : cardSet.lowContrast;
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

        GiftCard(playerObj, cardObj);
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
        AnimationUtilities.Lerp(card, card.position, transform.position, discardTime);
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
        AnimationUtilities.Lerp(card, card.position, discardLocation, discardTime);
    }

    public int GetCardPosition(GameObject original)
    {
        for (int i = 0; i < cards.Count; i++)
            if (original.name == cards[i].name)
                return i;
        return 0;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
