using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackjackGameManager : CardGameManager, IDataPersistence
{
    public static BlackjackGameManager Instance { get; private set; }

    // --- UI ---
    [Space(12)]
    [SerializeField] private Button[] _buttons;
    [SerializeField] private WinTextDisplay _winTextDisplay;
    private readonly Dictionary<Player, BlackjackPlayerState> _blackjackStates = new();

    // --- Internal Data ---
    private int _roundsPlayed = 0;
    private List<int> _playerInitialWins = new();

    // --- Conditions ---

    public bool PlayersActive => CurrentPhase == Phase.PlayerTurn;

    // --- Events ---

    #region Set Up
    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    protected override void InitGame()
    {
        base.InitGame();
        foreach (var player in TableHandler.Players)
        {
            if (_context.PlayerMap.TryGetValue(player, out var playerObj))
                JokerBank.Instance.GiftJoker(playerObj);
        }
        StartPhase(Phase.Clear);
    }

    protected override void SetPlayerData()
    {
        base.SetPlayerData();

        foreach (var player in TableHandler.Players)
        {
            _blackjackStates[player] = new();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (Instance == this)
            Instance = null;
    }

    #region Data Saving
    public void LoadData(GameData data)
    {
        if (!DataPersistenceManager.Instance.ResumeGame) return;

        for (int i = 0; i < TableHandler.Players.Length && i < data.blackjackScores.Length; i++)
        {
            _blackjackStates[TableHandler.GetPlayer(i)].Scores = data.blackjackScores[i].score;
            _blackjackStates[TableHandler.GetPlayer(i)].Wins = data.blackjackScores[i].wins;
        }
    }

    public void SaveData(ref GameData data)
    {
        data.blackjackScores = new GameData.BlackjackScore[_blackjackStates.Count];

        for (int i = 0; i < data.blackjackScores.Length; i++)
        {
            data.blackjackScores[i].score = _blackjackStates[TableHandler.GetPlayer(i)].Scores;
            data.blackjackScores[i].wins = _blackjackStates[TableHandler.GetPlayer(i)].Wins;
        }

        data.blackjackGames += _roundsPlayed;
        // TODO: add data.blackjackWins to the identified player
    }
    #endregion
    #endregion

    #region Runtime
    protected override void Update()
    {
        if (IsWaitingForSetup)
            return;

        foreach (var button in _buttons)
            button.interactable = TableHandler.PlayerTurn != 0 && CurrentPhase == Phase.PlayerTurn;
    }

    protected override void ResetGame()
    {
        Reshuffle();
    }

    protected override IEnumerator Deal()
    {
        yield return StartCoroutine(base.Deal());
        TableHandler.SetPlayerTurn(TableHandler.GetDealer());
        foreach (var player in TableHandler.Players)
        {
            if (_context.PlayerMap.TryGetValue(player, out var playerObj))
                JokerBank.Instance.GiftPityJoker(playerObj);
        }
    }

    protected override void DealPhase()
    {
        StartCoroutine(Deal());
    }

    #region Player Turn
    protected override void StartPlayerTurn()
    {
        // If it is Dealer's turn, then don't enact their first turn
        if (TableHandler.PlayerTurn == 0)
        {
            if (IsDealerTurn)
            {
                StartPhase(Phase.RoundEnd);
                IsDealerTurn = false;
                return;
            }
            IsDealerTurn = true;
            // Show Dealer's first card
            Players[0].cards[0].Reveal();
            Players[0].cards[1].Hide();
        }

        TableHandler.NextPlayerTurn();
        _blackjackStates[TableHandler.GetPlayerWhoseTurn()].Scores = BlackjackScorer.GetPlayerScore(Players[TableHandler.PlayerTurn]);
    }

    public void EndPlayerTurn()
    {
        OnPhaseComplete?.Invoke();
    }
    #endregion

    protected override void EndRound()
    {
        _roundsPlayed++;
        _winTextDisplay.DisplayWinner(BlackjackScorer.GetWinner(Players.ToArray()));
        IncrementWinsTally(BlackjackScorer.GetWinnerIndex(Players.ToArray()));
    }

    public BlackjackPlayerState GetState(Player player)
    {
        return _blackjackStates[player];
    }

    protected override void ClearHands()
    {
        base.ClearHands();
        _winTextDisplay.HideWinText();
        foreach (var text in _blackjackStates.Values)
        {
            text.Scores = 0;
            text.IsBust = false;
        }
    }

    /// <summary>
    /// Increments the Player's <c>wins</c> by 1
    /// </summary>
    /// <param name="player"></param>
    public void IncrementWinsTally(int player)
    {
        if (player >= 0)
            _blackjackStates[TableHandler.GetPlayer(player)].Wins++;
    }
    #endregion

    #region External Event Subscribers
    public void HitMe()
    {
        PlayerObject player = Players[TableHandler.PlayerTurn];
        player.data.Hand.Add(DeckHandler.DealRandomCard());
        player.SetHand(player.data.Hand, player.hand, player.cards);
        player.SetCards();
        player.RevealHand();
        _blackjackStates[player.data].Scores = BlackjackScorer.GetPlayerScore(player);
        if (!BlackjackScorer.CanHit(player))
            StartPhase(Phase.PlayerTurn);
    }

    public void Stay()
    {
        PlayerObject player = Players[TableHandler.PlayerTurn];
        _blackjackStates[player.data].Scores = BlackjackScorer.GetPlayerScore(player);
        StartPhase(Phase.PlayerTurn);
    }
    #endregion
}
