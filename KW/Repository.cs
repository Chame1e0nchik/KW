public interface IAccountRepository
{
    void Create(Account account);
    Account ReadById(int id);
    List<Account> ReadAll();
    void Update(Account account);
    void Delete(int id);
}

public interface IGameRecordRepository
{
    void Create(GameRecord gameRecord);
    GameRecord ReadById(int id);
    List<GameRecord> ReadAll();
    List<GameRecord> ReadByPlayerId(int playerId);
    void Update(GameRecord gameRecord);
    void Delete(int id);
}