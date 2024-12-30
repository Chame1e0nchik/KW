public abstract class GameType
{
    public enum GameResult
    {
        Win,
        Lose
    }

    protected static Random RandomGenerator = new();
    public string GameName = "";

    public abstract int PlayGame(Account player1, Account player2, GameResult result);

    protected enum GameCategory
    {
        Standard,
        SingleRating,
        Training
    }
}

public class SingleRatingGame : GameType
{
    public SingleRatingGame()
    {
        GameName = GameCategory.SingleRating.ToString();
    }

    public override int PlayGame(Account player1, Account player2, GameResult result)
    {
        var ratingChange = RandomGenerator.Next(29, 32);
        if (result == GameResult.Win)
            player2.CurrentRating -= player2.RatingMultiplier * ratingChange;
        else
            player2.CurrentRating += player2.RatingMultiplier * ratingChange;
            
        if (player1.CurrentRating < 0) 
        {
            player1.CurrentRating = 0;
        }
        if (player2.CurrentRating < 0) 
        {
            player2.CurrentRating = 0;
        }
        return ratingChange;
        
    }
}

public class TrainingGame : GameType
{
    public TrainingGame()
    {
        GameName = GameCategory.Training.ToString();
    }

    public override int PlayGame(Account player1, Account player2, GameResult result)
    {
        return 0;
    }
}

public class StandardGame : GameType
{
    public StandardGame()
    {
        GameName = GameCategory.Standard.ToString();
    }

    public override int PlayGame(Account player1, Account player2, GameResult result)
    {
        var ratingChange = RandomGenerator.Next(29, 32);
        if (result == GameResult.Win)
        {
            player1.CurrentRating += player1.RatingMultiplier * ratingChange;
            player2.CurrentRating -= player2.RatingMultiplier * ratingChange;
        }
        else
        {
            player1.CurrentRating -= player1.RatingMultiplier * ratingChange;
            player2.CurrentRating += player2.RatingMultiplier * ratingChange;
        }
        if (player1.CurrentRating < 0) 
        {
            player1.CurrentRating = 0;
        }
        if (player2.CurrentRating < 0) 
        {
            player2.CurrentRating = 0;
        }
        return ratingChange;
        
    }
}
