public interface IAccountService
{
    void CreateAccount(Account account);
    List<Account> GetAllAccounts();
    Account? GetAccountById(int id);
}

public interface IGameService
{
    void RecordGame(GameRecord gameRecord);
    List<GameRecord> GetAllGames();
    List<GameRecord> GetGamesByPlayerId(int playerId);
    int GetNextGameID();
}
