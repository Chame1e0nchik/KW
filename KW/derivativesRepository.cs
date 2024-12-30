public class accountRepository : IAccountRepository
{
    private readonly DbContext _context;

    public accountRepository(DbContext context)
    {
        _context = context;
    }

    public void Create(Account account)
    {
        _context.Players.Add(account);
    }

public Account ReadById(int id)
{
    foreach (var player in _context.Players)
    {
        if (player.Id == id)
        {
            return player;
        }
    }

    throw new InvalidOperationException("Account not found");
}


    public List<Account> ReadAll()
    {
        return _context.Players;
    }

    public void Update(Account account)
    {
        var existing = ReadById(account.Id);
        existing.CurrentRating = account.CurrentRating;
    }

    public void Delete(int id)
    {
        var account = ReadById(id);
        _context.Players.Remove(account);
    }
}


public class GameRecordRepository : IGameRecordRepository
{
    private readonly DbContext _context;
    private static int _gameCounter = 1;

    public GameRecordRepository(DbContext context)
    {
        _context = context;
    }

    public void Create(GameRecord gameRecord)
    {
        gameRecord.SetGameID(_gameCounter++);
        _context.Games.Add(gameRecord);
    }

public GameRecord ReadById(int gameId)
{
    foreach (var game in _context.Games)
    {
        if (game.GameID == gameId)
        {
            return game;
        }
    }
    throw new InvalidOperationException("Game record not found");
}

    public List<GameRecord> ReadAll()
    {
        return _context.Games;
    }

    public List<GameRecord> ReadByPlayerId(int playerId)
    {
        return _context.Games.Where(g => g.Player1.Id == playerId || g.Player2.Id == playerId).ToList();
    }

    public void Update(GameRecord gameRecord)
    {
        var existing = ReadById(gameRecord.GameID);
        throw new NotImplementedException("Update logic not implemented yet.");
    }

    public void Delete(int gameId)
    {
        var gameRecord = ReadById(gameId);
        _context.Games.Remove(gameRecord);
    }
}