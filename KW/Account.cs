public abstract class Account
{
    private static int _gameCounter;
    private static readonly List<GameRecord> GlobalGameHistory = new();
    protected AccountLevel Level;
    public int RatingMultiplier;
    public int WinsInRow {get; set;}
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public int CurrentRating { get; set; }
    public List<GameRecord> PersonalGameHistory { get; private set; } = new List<GameRecord>();

    public int TotalGames => PersonalGameHistory.Count;
    public virtual string AccountType => "";
    public int Wins
    {
        get
        {
            int count = 0;
            foreach (var g in PersonalGameHistory)
            {
                if ((g.Player1 == this && g.Result == "Win") || (g.Player2 == this && g.Result == "Lose"))
                {
                    count++;
                }
            }
            return count;
        }
    }

    public int Losses
    {
        get
        {
            int count = 0;
            foreach (var g in PersonalGameHistory)
            {
                if ((g.Player1 == this && g.Result == "Lose") || (g.Player2 == this && g.Result == "Win"))
                {
                    count++;
                }
            }
            return count;
        }
    }

    public void UpdateStatistics() {}
    
    public void Win(Account opponent, GameType gameType)
    {
        var ratingChange = gameType.PlayGame(this, opponent, GameType.GameResult.Win);
        if (Level == AccountLevel.Easy && ++WinsInRow == 3)
        {
            CurrentRating += 100;
            WinsInRow = 0;
        }

        var record = new GameRecord(++_gameCounter, gameType.GameName, this, opponent, "Win", ratingChange);
        GlobalGameHistory.Add(record);
        PersonalGameHistory.Add(record);

        UpdateStatistics();
    }

    public void Lose(Account opponent, GameType gameType)
    {
        var ratingChange = gameType.PlayGame(this, opponent, GameType.GameResult.Lose);
        if (Level == AccountLevel.Easy) WinsInRow = 0;

        var record = new GameRecord(++_gameCounter, gameType.GameName, this, opponent, "Lose", ratingChange);
        GlobalGameHistory.Add(record);
        PersonalGameHistory.Add(record);

        UpdateStatistics();
    }

    protected enum AccountLevel
    {
        Easy,
        Standard,
        Hardcore
    }
}
