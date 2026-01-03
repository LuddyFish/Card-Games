using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CardGameManager : MonoBehaviour
{
    // --- Table Properties ---
    [SerializeField] private CardGameContext _context;

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

    // --- Players ---
    [HideInInspector] public List<PlayerObject> Players { get; private set; } = new();
    private int _minPlayerCount = 2;
    public int MinPlayerCount
    {
        get { return _minPlayerCount; }
        set { _minPlayerCount = value; }
    }

    // --- Internal Data ---
    [SerializeField] private int _startingCardCount = 5;
    [Tooltip("<b>False</b>: deal all cards simultaneously" +
        "\n<b>True</b>: deal all cards one at a time")]
    [SerializeField] private bool _dealSequentially = true;

    // --- Conditions ---
    private bool _isWaitingForSetup = true;
    protected bool IsWaitingForSetup => _isWaitingForSetup;
    private bool _isDealerTurn = false;
    protected bool IsDealerTurn
    {
        get { return _isDealerTurn; }
        set { _isDealerTurn = value; }
    }

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
    }

    protected virtual void SetDataVariables()
    {
        DeckHandler = new(set: _cardSet);
        _context.Deck = DeckHandler;

        var cardManager = Instantiate(_cardManagerPrefab);
        var cardbox = cardManager.GetComponent<Cardbox>();
        cardbox.cardSet = _cardSet;
        OnGameLoaded += cardbox.Init;

        DataPersistenceManager.Instance.Init();
    }

    protected virtual void AddEventSubscribers()
    {
        OnShuffle += DeckHandler.NewDeck;
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
    protected virtual void Deal()
    {
        if (!_dealSequentially)
        {
            DeckHandler.DealContinuous(TableHandler, true);
            OnDeal?.Invoke();
        }
        else
            StartCoroutine(DealSequential(_startingCardCount));
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
                DeckHandler.DealSegmented(TableHandler);
                yield return new WaitUntil(() => dealt);
            }

        Cardbox.Instance.OnDealAnimationCompletion -= Handler;

        OnDeal?.Invoke();
    }

    /// <summary>
    /// Discard all cards from every player's hands
    /// </summary>
    protected virtual void ClearHands()
    {
        OnReset?.Invoke();
    }

    public abstract int GetPlayerScore(PlayerObject player);
    #endregion
}
