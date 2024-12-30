public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public void CreateAccount(Account account)
    {
        _accountRepository.Create(account);
    }

    public List<Account> GetAllAccounts()
    {
        return _accountRepository.ReadAll();
    }

    public Account? GetAccountById(int id)
    {
        try
        {
            return _accountRepository.ReadById(id);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"Account with ID {id} not found.");
            return null;
        }
    }
}

public class GameService : IGameService
{
    private static int _gameCounter = 1;

    private readonly IGameRecordRepository _gameRepository;

    public GameService(IGameRecordRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public void RecordGame(GameRecord gameRecord)
    {
        _gameRepository.Create(gameRecord);
    }

    public List<GameRecord> GetAllGames()
    {
        return _gameRepository.ReadAll();
    }

    public List<GameRecord> GetGamesByPlayerId(int playerId)
    {
        return _gameRepository.ReadByPlayerId(playerId);
    }

    public int GetNextGameID()
    {
        return _gameCounter++;
    }
}
