using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CardGameManager : PhaseController
{
    // --- Table Properties ---
    [SerializeField] protected CardGameContext _context;
    protected DataPersistenceManager _persistenceManager => DataPersistenceManager.Instance;

    private Table _table;
    public Table TableHandler {
        get { return _table; }
        protected set { _table = value; }
    }
    private Deck _deck;
    public Deck DeckHandler {
        get { return _deck; }
        protected set { _deck = value; }
    }

    [SerializeField] private CardDeckSet _cardSet;

    [Tooltip("This GameObject must contain a \"Cardbox\" script")]
    [SerializeField] private GameObject _cardManagerPrefab;
    [Tooltip("This GameObject must contain a \"JokerBank\" script")]
    [SerializeField] private GameObject _jokerManagerPrefab;

    // --- Players ---
    [HideInInspector] public List<PlayerObject> Players { get; private set; } = new();
    [HideInInspector] public int MinPlayerCount = 2;

    // --- Internal Data ---
    [SerializeField] private int _startingCardCount = 5;
    [Tooltip("<b>False</b>: deal all cards simultaneously" +
             "\n<b>True</b>: deal all cards one at a time")]
    [SerializeField] private bool _dealSequentially = true;
    public int startingJokerCount = 2;
    public int maxJokerCount = 3;

    // --- Conditions ---
    private bool _isWaitingForSetup = true;
    protected bool IsWaitingForSetup => _isWaitingForSetup;
    protected bool IsDealerTurn = false;

    // --- Events ---
    public Action OnGameLoaded;
    public Action OnDeal;
    public Action OnShuffle;
    public Action OnReset;

    #region Set Up
    protected virtual void Awake()
    {
        if (_context.ActiveGame == null)
            _context.SetGame(this);
        else
            Destroy(this);
    }

    protected virtual IEnumerator Start()
    {
        _isWaitingForSetup = true;
        yield return new WaitUntil(() => Players.Count >= MinPlayerCount);
        InitGame();
        _isWaitingForSetup = false;
    }

    protected virtual void InitGame()
    {
        SetPlayerData();
        SetDataVariables();
        AddEventSubscribers();

        OnGameLoaded?.Invoke();
    }

    protected virtual void SetPlayerData()
    {
        foreach (var player in Players)
            _context.PlayerMap.Add(player.data, player);
        TableHandler = new(
            Players: _context.PlayerMap.Keys.ToArray(),
            StartingCardCount: _startingCardCount
        );
        _context.Table = TableHandler;
        _persistenceManager.RegisterDataPersistenceObject(TableHandler);
    }

    protected virtual void SetDataVariables()
    {
        DeckHandler = new(
            set: _cardSet,
            isAvailable: card => _context.CardMap.TryGetValue(card, out var obj) && !obj.inHand
        );
        _context.Deck = DeckHandler;
        _persistenceManager.RegisterDataPersistenceObject(DeckHandler);

        var cardManager = Instantiate(_cardManagerPrefab);
        var cardbox = cardManager.GetComponent<Cardbox>();
        cardbox.cardSet = _cardSet;
        OnGameLoaded += cardbox.Init;
        OnGameLoaded += () => cardbox.InitJokers(Players.Count, startingJokerCount);

        var jokerManager = Instantiate(_jokerManagerPrefab);

        _persistenceManager.Init();
    }

    protected virtual void AddEventSubscribers()
    {
        OnShuffle += DeckHandler.NewDeck;

        if (CardAudio.Instance != null)
        {
            DeckHandler.OnDeckShuffled += () => CardAudio.Instance.PlayDeckShuffle(CardAudio.Instance.sources[0]);
            DeckHandler.OnDeckSoftShuffled += () => CardAudio.Instance.PlayDeckShuffle(CardAudio.Instance.sources[0]);
            DeckHandler.OnBatchDealStart += () => CardAudio.Instance.PlayCardShuffle(CardAudio.Instance.sources[0]);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_context.ActiveGame == this)
            _context.ClearGame();
    }
    #endregion

    #region Other variables subscribing
    public void SetPlayer(PlayerObject player)
    {
        Players.Add(player);
        Debug.Log($"Added {player.name} to list of Players. Total: {Players.Count}");
    }

    public void SetPlayer(PlayerObject player, int priority)
    {
        Players.Insert(priority, player);
        Debug.Log($"Added {player.name} to list of Players at position {priority}. Total: {Players.Count}");
    }
    #endregion

    #region Runtime
    protected virtual void Update()
    {
        if (IsWaitingForSetup)
            return;
    }

    /// <summary>
    /// Refills the deck
    /// </summary>
    protected virtual void Reshuffle()
    {
        OnShuffle?.Invoke();
    }

    /// <summary>
    /// Deal cards to all players
    /// </summary>
    protected virtual IEnumerator Deal()
    {
        if (!_dealSequentially)
            DeckHandler.DealContinuous(TableHandler, true);
        else
            yield return StartCoroutine(DealSequential(_startingCardCount));

        OnDeal?.Invoke();
        IsDealerTurn = false;
    }

    protected IEnumerator DealSequential(int rounds)
    {
        bool dealt = false;
        void Handler() => dealt = true;
        Cardbox.Instance.OnDealAnimationCompletion += Handler;

        for (int i = 0; i < rounds; i++)
            for (int j = 0; j < TableHandler.Players.Length; j++)
            {
                dealt = false;
                DeckHandler.DealSegmented(TableHandler, i * TableHandler.Players.Length + j);
                yield return new WaitUntil(() => dealt);
            }

        Cardbox.Instance.OnDealAnimationCompletion -= Handler;
    }

    /// <summary>
    /// Discard all cards from every player's hands
    /// </summary>
    protected override void ClearHands()
    {
        OnReset?.Invoke();
    }
    #endregion
}
