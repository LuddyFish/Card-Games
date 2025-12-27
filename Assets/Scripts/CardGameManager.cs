using System;
using System.Collections;
using System.Collections.Generic;
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

    /// <summary>
    /// Prefab <b>must</b> include <see cref="Cardbox"/>
    /// </summary>
    [Tooltip("This GameObject must contain a \"Cardbox\" script")]
    [SerializeField] private GameObject _cardManagerPrefab;

    // --- Players ---
    [HideInInspector] public List<PlayerObject> Players { get; private set; } = new();
    protected List<Player> _playerDatas = new();
    private int _minPlayerCount = 2;
    public int MinPlayerCount
    {
        get { return _minPlayerCount; }
        set { _minPlayerCount = value; }
    }

    // --- Internal Data ---
    [SerializeField] private int _startingCardCount = 5;

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
            _playerDatas.Add(player.data);
        TableHandler = new(
            Players: _playerDatas.ToArray(),
            StartingCardCount: _startingCardCount
        );
        _context.Table = TableHandler;
    }

    protected virtual void SetDataVariables()
    {
        DeckHandler = new();
        _context.Deck = DeckHandler;

        var cardManager = Instantiate(_cardManagerPrefab);
        var cardbox = cardManager.GetComponent<Cardbox>();
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
        DeckHandler.Deal(TableHandler);
        OnDeal?.Invoke();
        IsDealerTurn = false;
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
