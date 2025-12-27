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
    [HideInInspector] public List<BlackjackScores> PlayerScores { get; private set; } = new();

    [SerializeField] private GameObject _pausedScreen;
    [HideInInspector] public bool isPaused = false;

    // --- Internal Data ---
    private int _roundsPlayed = 0;
    private List<int> _playerInitialWins = new();

    // --- Conditions ---
    private int _phase = 0;
    private bool _waitingForPhase = false;

    public bool PlayersActive => _phase == 2;

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
        StartPhase(4);
    }

    protected override void SetPlayerData()
    {
        base.SetPlayerData();

        foreach (var scorer in PlayerScores)
        {
            scorer.Scores = 0;
            scorer.Wins = 0;
            scorer.ToggleBust(false);
            _playerInitialWins.Add(scorer.Wins);
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

    #region Other variables subscribing
    public void SetScorer(BlackjackScores scorer)
    {
        PlayerScores.Add(scorer);
        Debug.Log($"Added {scorer.name} to list of Scorers. Total: {PlayerScores.Count}");
    }

    public void SetScorer(BlackjackScores scorer, int priority)
    {
        PlayerScores.Insert(priority, scorer);
        Debug.Log($"Added {scorer.name} to list of Scorers at position {priority}. Total: {PlayerScores.Count}");
    }
    #endregion

    #region Data Saving
    public void LoadData(GameData data)
    {
        for (int i = 0; i < PlayerScores.Count && i < data.blackjackScores.Length; i++)
        {
            PlayerScores[i].Scores = data.blackjackScores[i].score;
            PlayerScores[i].Wins = data.blackjackScores[i].wins;
        }
    }

    public void SaveData(ref GameData data)
    {
        data.SaveBlackjackData();
    }

    public void LoadData(PlayerGameStats data)
    {

    }

    public void SaveData(ref PlayerGameStats data)
    {
        data.blackjackGames += _roundsPlayed;
        // TODO: add stats.blackjackWins to the identified player
    }
    #endregion

    #region Runtime
    protected override void Update()
    {
        if (IsWaitingForSetup)
            return;

        if (!_waitingForPhase)
        {
            switch (_phase)
            {
                case 0:
                    _waitingForPhase = true;
                    DelayStartPhase(1, 0.5f);
                    break;
                case 1:
                    _waitingForPhase = true;
                    DelayStartPhase(2, 0.2f);
                    break;
                case 2:
                    break;
                case 3:
                    _waitingForPhase = true;
                    DelayStartPhase(4, 3f);
                    break;
                case 4:
                    _waitingForPhase = true;
                    if (DeckHandler.NotEnoughCards(TableHandler.Players.Length * TableHandler.StartingCardCount))
                        DelayStartPhase(0, 0.75f);
                    else
                        DelayStartPhase(1, 0.2f);
                    break;
            }
        }

        foreach (var button in _buttons)
            button.interactable = TableHandler.PlayerTurn != 0;
    }

    /// <summary>
    /// Enacts a specified stage of the game
    /// </summary>
    /// <param name="phase">
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
    /// </param>
    private void StartPhase(int phase)
    {
        _phase = phase;
        Debug.Log("Enacting phase: " + phase);
        switch (phase)
        {
            case 0:
                Reshuffle();
                break;
            case 1:
                Deal();
                TableHandler.SetPlayerTurn(TableHandler.GetDealer());
                break;
            case 2:
                // If is dealer's turn and don't enact their first turn
                if (TableHandler.PlayerTurn == 0)
                {
                    if (IsDealerTurn) {
                        StartPhase(3);
                        return;
                    }
                    IsDealerTurn = true;
                    Players[0].cards[0].Reveal();
                    Players[0].cards[1].Hide();
                }
                TableHandler.NextPlayerTurn();
                PlayerScores[TableHandler.PlayerTurn].Scores = GetPlayerScore(Players[TableHandler.PlayerTurn]);
                break;
            case 3:
                DisplayWinner(GetWinner());
                IncrementWinsTally(GetWinnerIndex());
                break;
            case 4:
                ClearHands();
                break;
        }
    }

    /// <summary>
    /// Enacts a specific stage of the game after <paramref name="t"/> seconds
    /// </summary>
    /// <param name="phase">Refer to <see cref="StartPhase(int)"/> for phase numbers</param>
    /// <param name="t">Time to wait</param>
    /// <returns></returns>
    private Coroutine DelayStartPhase(int phase, float t) => StartCoroutine(DelayPhase(phase, t));

    private IEnumerator DelayPhase(int phase, float t)
    {
        yield return new WaitForSeconds(t);
        StartPhase(phase);
        _waitingForPhase = false;
    }

    protected override void ClearHands()
    {
        base.ClearHands();
        HideWinText();
        foreach (var text in PlayerScores)
        {
            text.Scores = 0;
            text.ToggleBust(false);
        }
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
        if (player >= 0) PlayerScores[player].Wins = PlayerScores[player].Wins + 1;
    }

    public void HideWinText()
    {
        _winTextBox.SetActive(false);
    }

    /// <summary>
    /// Checks the players score
    /// </summary>
    /// <param name="player"></param>
    /// <returns>
    /// <c>0 =</c> greater than 21<br/> 
    /// <c>1 =</c> less than 21<br/> 
    /// <c>2 =</c> equals 21
    /// </returns>
    public int CanHit(PlayerObject player)
    {
        int score = GetPlayerScore(player);
        if (score > 21)
            return 0;
        else if (score < 21)
            return 1;
        else
            return 2;
    }
    #endregion

    #region External Event Subscribers
    public void HitMe()
    {
        Players[TableHandler.PlayerTurn].data.Hand.Add(DeckHandler.DealRandomCard());
        Players[TableHandler.PlayerTurn].SetHand();
        Players[TableHandler.PlayerTurn].RevealHand();
        PlayerScores[TableHandler.PlayerTurn].Scores = GetPlayerScore(Players[TableHandler.PlayerTurn]);
        if (CanHit(Players[TableHandler.PlayerTurn]) != 1)
            StartPhase(2);
    }

    public void Stay()
    {
        PlayerScores[TableHandler.PlayerTurn].Scores = GetPlayerScore(Players[TableHandler.PlayerTurn]);
        StartPhase(2);
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
