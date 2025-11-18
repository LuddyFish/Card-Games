using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackjackGameManager : MonoBehaviour
{
    public static BlackjackGameManager Instance;

    public PlayerObject dealer;
    public PlayerObject player;
    [HideInInspector] public PlayerObject[] Players { get; private set; }
    [HideInInspector] public Player[] PlayerDatas { get; private set; }
    private Cardbox Box => Cardbox.Instance;

    [Space(12)]
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject winTextBox;
    private Text winText;
    [SerializeField] private BlackjackScores dealerScore;
    [SerializeField] private BlackjackScores[] playersScore;
    private BlackjackScores[] playerScores;

    private int phase = 0;
    private bool dealerTurn = false;
    private bool waitingforPhase = false;

    public bool PlayersActive => phase == 2;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    void Start()
    {
        SetPlayers();
        InstantiateCards();
        SetOtherVariables();

        StartPhase(0);
    }

    void SetPlayers()
    {
        Players = new PlayerObject[2];
        PlayerDatas = new Player[2];
        Players[0] = dealer;
        Players[1] = player;
        for (int i = 0; i < Players.Length; i++)
            PlayerDatas[i] = Players[i].Data;

        Table.NewTable(PlayerDatas);
        Table.playerTurn = 0;
    }

    void InstantiateCards()
    {
        Table.startingCardCount = 2;
        Deck.InitDeck();
        Box.Init();
    }

    void SetOtherVariables()
    {
        winText = winTextBox.GetComponentInChildren<Text>();
        HideWinText();
        
        playerScores = new BlackjackScores[playersScore.Length + 1];
        playerScores[0] = dealerScore;
        playerScores[1] = playersScore[0];
        foreach (var item in playerScores)
        {
            item.SetScore(0);
            item.SetWins(0);
            item.ToggleBust(false);
        }
    }

    void Update()
    {
        if (!waitingforPhase)
        {
            switch (phase)
            {
                case 0:
                    StartPhase(1);
                    break;
                case 1:
                    StartPhase(2);
                    break;
                case 2:
                    break;
                case 3:
                    waitingforPhase = true;
                    StartCoroutine(DelayStartPhase(4, 3f));
                    break;
                case 4:
                    waitingforPhase = true;
                    StartCoroutine(Deck.NotEnoughCards() ? DelayStartPhase(0, 0.75f) : DelayStartPhase(1, 0.2f));
                    break;
            }
        }

        foreach (var button in buttons)
        {
            button.interactable = Table.playerTurn != 0;
        }
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
    void StartPhase(int phase)
    {
        this.phase = phase;
        Debug.Log("Enacting phase: " + phase);
        switch (phase)
        {
            case 0:
                Reshuffle();
                break;
            case 1:
                Deal();
                Table.playerTurn = Table.GetDealer();
                break;
            case 2:
                // If is dealer's turn and don't enact their first turn
                if (Table.playerTurn == 0)
                {
                    if (dealerTurn) {
                        StartPhase(3);
                        return;
                    }
                    dealerTurn = true;
                    dealer.cards[0].Reveal();
                    dealer.cards[1].Hide();
                }
                Table.NextPlayerTurn();
                playerScores[Table.playerTurn].SetScore(GetPlayerScore(Players[Table.playerTurn]));
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
    IEnumerator DelayStartPhase(int phase, float t)
    {
        yield return new WaitForSeconds(t);
        StartPhase(phase);
        waitingforPhase = false;
    }

    /// <summary>
    /// Refills the deck
    /// </summary>
    void Reshuffle()
    {
        Deck.NewDeck();
        Box.ReturnCardsToDeck();
    }

    /// <summary>
    /// Deal cards to all players
    /// </summary>
    void Deal()
    {
        Deck.Deal();
        dealer.SetHand();
        player.SetHand();
        dealerTurn = false;
    }

    /// <summary>
    /// Discard all cards from every player's hands
    /// </summary>
    void ClearHands()
    {
        dealer.DiscardCards();
        player.DiscardCards();
        HideWinText();
        foreach (var text in playerScores)
        {
            text.SetScore(0);
            text.ToggleBust(false);
        }
    }

    /// <summary>
    /// Calculates the Player's total score
    /// </summary>
    /// <param name="player"></param>
    /// <returns>The sum value of all cards</returns>
    public int GetPlayerScore(PlayerObject player)
    {
        int score = 0;
        bool ace = false;
        foreach (var card in player.cards)
        {
            int value = Card.BlackjackValue((Card.Ranks)card.card.Rank);
            if (value == 1)
            {
                ace = true;
            }
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

        for (int i = 0; i < Players.Length; i++)
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
        winTextBox.SetActive(true);
    }

    /// <summary>
    /// Increments the Player's <c>wins</c> by 1
    /// </summary>
    /// <param name="player"></param>
    public void IncrementWinsTally(int player)
    {
        if (player >= 0) playerScores[player].SetWins(playerScores[player].GetWins() + 1);
    }

    public void HideWinText()
    {
        winTextBox.SetActive(false);
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


    public void HitMe()
    {
        PlayerDatas[Table.playerTurn].Hand.Add(Deck.DealRandomCard());
        Players[Table.playerTurn].SetHand();
        Players[Table.playerTurn].RevealHand();
        playerScores[Table.playerTurn].SetScore(GetPlayerScore(Players[Table.playerTurn]));
        if (CanHit(Players[Table.playerTurn]) != 1)
            StartPhase(2);
    }

    public void Stay()
    {
        playerScores[Table.playerTurn].SetScore(GetPlayerScore(Players[Table.playerTurn]));
        StartPhase(2);
    }
}
