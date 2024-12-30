public static class GlobalGameHistory
{
    public static List<GameRecord> AllGames = new List<GameRecord>();
    
    public static void Add(GameRecord gameRecord)
    {
        AllGames.Add(gameRecord);
    }
}
