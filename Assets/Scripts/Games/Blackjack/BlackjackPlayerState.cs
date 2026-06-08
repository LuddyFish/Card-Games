public class BlackjackPlayerState
{
    public int Scores;
    public int Wins;
    public bool IsBust => Scores > 21;
}
