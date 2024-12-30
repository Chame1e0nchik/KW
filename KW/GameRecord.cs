public class GameRecord
{
    public int GameID { get; set; }
    public string GameName { get; }
    public Account Player1 { get; }
    public Account Player2 { get; }
    public string Result { get; }
    public int RatingChange { get; }

    public GameRecord(int gameID, string gameName, Account player1, Account player2, string result, int ratingChange)
    {
        GameID = gameID;
        GameName = gameName;
        Player1 = player1;
        Player2 = player2;
        Result = result;
        RatingChange = ratingChange;
    }

    public void SetGameID(int gameID)
    {
        if (GameID == 0)
        {
            GameID = gameID;
        }
        else
        {
            throw new InvalidOperationException("GameID can only be set once.");
        }
    }
}