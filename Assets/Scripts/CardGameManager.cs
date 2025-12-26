using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardGameManager : MonoBehaviour
{
    // --- Table Properties ---
    [SerializeField] private CardGameContext _context;

    public Deck DeckHandler { get; protected set; }
    public Table TableHandler { get; protected set; }

    /// <summary>
    /// Prefab <b>must</b> include <see cref="Cardbox"/>
    /// </summary>
    [Tooltip("This GameObject must contain a \"Cardbox\" script")]
    [SerializeField] private GameObject _cardManagerPrefab;

    // --- Players ---
    [HideInInspector] public List<PlayerObject> Players { get; private set; } = new();
    protected List<Player> _playerDatas = new();

    // --- Internal Data ---
    [SerializeField] private int _startingCardCount = 5;

    // --- Conditions ---
    private bool _isDealerTurn = false;
    protected bool IsDealerTurn
    {
        get => _isDealerTurn;
        set => _isDealerTurn = value;
    }

    // --- Events ---
    public Action onDeal;
    public Action onShuffle;
    public Action onReset;

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
        yield return new WaitForEndOfFrame();
        SetPlayerData();
        SetDataVariables();
        AddEventSubscribers();
    }

    protected virtual void SetPlayerData()
    {
        foreach (var player in Players)
            _playerDatas.Add(player.data);
        TableHandler = new(
            players: _playerDatas.ToArray(),
            startingCardCount: _startingCardCount
        );
    }

    protected virtual void SetDataVariables()
    {
        DeckHandler = new();
        Instantiate(_cardManagerPrefab);
        DataPersistenceManager.Instance.Init();
    }

    protected virtual void AddEventSubscribers()
    {
        onShuffle += DeckHandler.NewDeck;
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
        Debug.Log($"Added {player.name} to list of Players");
    }

    public void SetPlayer(PlayerObject player, int priority)
    {
        Players.Insert(priority, player);
        Debug.Log($"Added {player.name} to list of Players at position {priority}");
    }
    #endregion

    /// <summary>
    /// Refills the deck
    /// </summary>
    protected virtual void Reshuffle()
    {
        onShuffle?.Invoke();
    }

    /// <summary>
    /// Deal cards to all players
    /// </summary>
    protected virtual void Deal()
    {
        DeckHandler.Deal(TableHandler);
        onDeal?.Invoke();
        IsDealerTurn = false;
    }

    /// <summary>
    /// Discard all cards from every player's hands
    /// </summary>
    protected virtual void ClearHands()
    {
        onReset?.Invoke();
    }

    public abstract int GetPlayerScore(PlayerObject player);
}
