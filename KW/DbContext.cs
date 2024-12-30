public class DbContext
{
    private int _accountIdCounter = 1;
    private int _gameIdCounter = 1;

    public List<Account> Players { get; set; } = new();
    public List<GameRecord> Games { get; set; } = new();

    public int GetNextPlayerId()
    {
        return _accountIdCounter++;
    }

    public int GetNextGameId()
    {
        return _gameIdCounter++;
    }

    public DbContext()
    {
        var alice = new RegularAccount("Alice") { Id = _accountIdCounter++ };
        var bob = new RegularAccount("Bob") { Id = _accountIdCounter++ };
        var charlie = new HardcoreAccount("Charlie") { Id = _accountIdCounter++ };
        var diana = new HardcoreAccount("Diana") { Id = _accountIdCounter++ };
        var eve = new BonusAccount("Eve") { Id = _accountIdCounter++ };
        var frank = new BonusAccount("Frank") { Id = _accountIdCounter++ };

        Players.Add(alice);
        Players.Add(bob);
        Players.Add(charlie);
        Players.Add(diana);
        Players.Add(eve);
        Players.Add(frank);
    }
}