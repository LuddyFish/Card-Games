using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackjackGameManager : CardGameManager, IDataPersistence<GameData>, IDataPersistence<PlayerGameStats>
{
    public static BlackjackGameManager Instance { get; private set; }

    // --- UI ---
    [Space(12)]
    [SerializeField] private Button[] _buttons;
    [SerializeField] private GameObject _winTextBox;
    private Text winText;
    private readonly Dictionary<Player, BlackjackPlayerState> _blackjackStates = new();

    [SerializeField] private GameObject _pausedScreen;
    [HideInInspector] public bool isPaused = false;

    // --- Internal Data ---
    private int _roundsPlayed = 0;
    private List<int> _playerInitialWins = new();

    // --- Conditions ---
    /// <summary>
    /// <list type="bullet">
    /// <item>
    ///     <term>0</term>
    ///     <description>Reset the game</description>
    /// </item>
    /// <item>
    ///     <term>1</term>
    ///     <description>Deal a set of cards</description>
    /// </item>
    /// <item>
    ///     <term>2</term>
    ///     <description>Go to next Player's turn</description>
    /// </item>
    /// <item>
    ///     <term>3</term>
    ///     <description>Determine the winner</description>
    /// </item>
    /// <item>
    ///     <term>4</term>
    ///     <description>Clear Hand</description>
    /// </item>
    /// </list>
    /// </summary>
    private enum Phase
    {
        Reset,
        Deal,
        PlayerTurn,
        RoundEnd,
        Clear
    }
    private Phase _phase = Phase.Reset;

    public bool PlayersActive => _phase == Phase.PlayerTurn;

    // --- Events ---
    private Action OnPhaseComplete;

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

    protected override void SetDataVariables()
    {
        winText = _winTextBox.GetComponentInChildren<Text>();
        HideWinText();
        
        base.SetDataVariables();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (Instance == this)
            Instance = null;
    }
    #endregion

    #region Data Saving
    public void LoadData(GameData data)
    {
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
    }

    public void LoadData(PlayerGameStats data)
    {

    }

    public void SaveData(ref PlayerGameStats data)
    {
        data.blackjackGames += _roundsPlayed;
        // TODO: add data.blackjackWins to the identified player
    }
    #endregion

    #region Runtime
    protected override void Update()
    {
        if (IsWaitingForSetup)
            return;

        foreach (var button in _buttons)
            button.interactable = TableHandler.PlayerTurn != 0;
    }

    #region Phase Logic
    private void AdvancePhase()
    {
        _phase = GetNextPhase(_phase);
        Debug.Log("Next phase is: " + _phase);
        DelayStartPhase(_phase, GetPhaseDelayTime(_phase));
    }

    private Phase GetNextPhase(Phase current)
    {
        return current switch
        {
            Phase.Reset =>      Phase.Deal,
            Phase.Deal =>       Phase.PlayerTurn,
            Phase.PlayerTurn => Phase.RoundEnd,
            Phase.RoundEnd =>   Phase.Clear,
            Phase.Clear =>      DeckHandler.NotEnoughCards(TableHandler.Players.Length * TableHandler.StartingCardCount) ? Phase.Reset : Phase.Deal,
            _ =>                Phase.Reset
        };
    }

    private float GetPhaseDelayTime(Phase current)
    {
        return current switch
        {
            Phase.Reset         => 0.75f,
            Phase.Deal          => 0.5f,
            Phase.PlayerTurn    => 0.2f,
            Phase.RoundEnd      => 0.1f,
            Phase.Clear         => 3f,
            _                   => 0f
        };
    }

    /// <summary>
    /// Enacts a specified stage of the game
    /// </summary>
    /// <param name="phase">Refer to <see cref="Phase"/> for phase numbers</param>
    private void StartPhase(Phase phase)
    {
        Debug.Log("Enacting phase: " + (int)phase);
        _phase = phase;

        OnPhaseComplete = null;

        switch (phase)
        {
            case Phase.Reset:
                OnPhaseComplete += AdvancePhase;
                Reshuffle();
                break;
            case Phase.Deal:
                OnPhaseComplete += AdvancePhase;
                StartCoroutine(Deal());
                break;
            case Phase.PlayerTurn:
                StartPlayerTurn();
                break;
            case Phase.RoundEnd:
                OnPhaseComplete += AdvancePhase;
                _roundsPlayed++;
                DisplayWinner(GetWinner());
                IncrementWinsTally(GetWinnerIndex());
                OnPhaseComplete?.Invoke();
                break;
            case Phase.Clear:
                OnPhaseComplete += AdvancePhase;
                ClearHands();
                break;
        }
    }

    /// <summary>
    /// Enacts a specific stage of the game after <paramref name="t"/> seconds
    /// </summary>
    /// <param name="phase">Refer to <see cref="Phase"/> for phase numbers</param>
    /// <param name="t">Time to wait</param>
    /// <returns></returns>
    private Coroutine DelayStartPhase(Phase phase, float t) => StartCoroutine(DelayPhase(phase, t));

    private IEnumerator DelayPhase(Phase phase, float t)
    {
        yield return new WaitForSeconds(t);
        StartPhase(phase);
    }
    #endregion

    protected override void Reshuffle()
    {
        base.Reshuffle();
        OnPhaseComplete?.Invoke();
    }

    protected override IEnumerator Deal()
    {
        yield return StartCoroutine(base.Deal());
        OnPhaseComplete?.Invoke();
        TableHandler.SetPlayerTurn(TableHandler.GetDealer());
    }

    #region Player Turn
    private void StartPlayerTurn()
    {
        // If it is Dealer's turn, then don't enact their first turn
        if (TableHandler.PlayerTurn == 0)
        {
            if (IsDealerTurn)
            {
                StartPhase(Phase.RoundEnd);
                return;
            }
            IsDealerTurn = true;
            // Show Dealer's first card
            Players[0].cards[0].Reveal();
            Players[0].cards[1].Hide();
        }

        TableHandler.NextPlayerTurn();
        _blackjackStates[TableHandler.GetPlayerWhoseTurn()].Scores = GetPlayerScore(Players[TableHandler.PlayerTurn]);
    }

    public void EndPlayerTurn()
    {
        OnPhaseComplete?.Invoke();
    }
    #endregion

    public BlackjackPlayerState GetState(Player player)
    {
        return _blackjackStates[player];
    }

    protected override void ClearHands()
    {
        base.ClearHands();
        HideWinText();
        foreach (var text in _blackjackStates.Values)
        {
            text.Scores = 0;
            text.IsBust = false;
        }
        OnPhaseComplete?.Invoke();
    }

    /// <summary>
    /// Calculates the Player's total score
    /// </summary>
    /// <param name="player"></param>
    /// <returns>The sum value of all cards</returns>
    public override int GetPlayerScore(PlayerObject player)
    {
        int score = 0;
        bool ace = false;
        foreach (var card in player.cards)
        {
            int value = Card.BlackjackValue((Card.Ranks)card.card.Rank);
            if (value == 1)
                ace = true;
            score += value;
        }
        if (ace && score + 10 <= 21) // Account for the fact that ace is a value of 1 OR 11
            score += 10;
        return score;
    }

    /// <summary>
    /// Select the Player with the highest legal score
    /// </summary>
    /// <returns>The winner</returns>
    public PlayerObject GetWinner()
    {
        PlayerObject winner = null;
        int highest = 0;

        foreach (var player in Players)
        {
            int score = GetPlayerScore(player);
            if (score <= 21 && score > highest)
            {
                highest = score;
                winner = player;
            }
        }

        return winner;
    }

    /// <summary>
    /// Select the Player with the highest legal score
    /// </summary>
    /// <returns>The winning Player's index position</returns>
    public int GetWinnerIndex()
    {
        int winner = -1;
        int highest = 0;

        for (int i = 0; i < Players.Count; i++)
        {
            int score = GetPlayerScore(Players[i]);
            if (score <= 21 && score > highest)
            {
                highest = score;
                winner = i;
            }
        }

        return winner;
    }

    /// <summary>
    /// Sets the Player's name on the <c>Winner</c> text box
    /// </summary>
    /// <param name="player"></param>
    public void DisplayWinner(PlayerObject player)
    {
        winText.text = player != null ? $"{player.name} Wins!" : "Everyone's bust...";
        _winTextBox.SetActive(true);
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

    public void HideWinText()
    {
        _winTextBox.SetActive(false);
    }

    /// <summary>
    /// Checks the players score if less than 21
    /// </summary>
    /// <param name="player"></param>
    /// <returns>Returns <c>TRUE</c> if the player has less than 21</returns>
    public bool CanHit(PlayerObject player)
    {
        if (GetPlayerScore(player) >= 21)
            return false;
        else
            return true;
    }
    #endregion

    #region External Event Subscribers
    public void HitMe()
    {
        PlayerObject player = Players[TableHandler.PlayerTurn];
        player.data.Hand.Add(DeckHandler.DealRandomCard());
        player.SetHand();
        player.SetCards();
        player.RevealHand();
        _blackjackStates[player.data].Scores = GetPlayerScore(player);
        if (!CanHit(player))
            StartPhase(Phase.PlayerTurn);
    }

    public void Stay()
    {
        PlayerObject player = Players[TableHandler.PlayerTurn];
        _blackjackStates[player.data].Scores = GetPlayerScore(player);
        StartPhase(Phase.PlayerTurn);
    }

    public void TogglePause(bool pause)
    {
        isPaused = pause;
        SetPause();
    }

    private void SetPause()
    {
        _pausedScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }
    #endregion
}
