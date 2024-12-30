using System.Reflection.Emit;
using System.Security.AccessControl;
using System.Security.Principal;

public interface ICommand
{
    void Execute();
    string GetInfo();
}

public class ListPlayersCommand : ICommand
{
    private readonly IAccountService _accountService;

    public ListPlayersCommand(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public void Execute()
    {
        var players = _accountService.GetAllAccounts();

        if (players == null || !players.Any())
        {
            Console.WriteLine("No players found.");
            return;
        }
        Console.WriteLine("+--------------------------------+");
        Console.WriteLine("|        List of players         |");
        foreach (var player in players)
        {
        Console.WriteLine("|________________________________|");
        Console.WriteLine($"| Player: {player.Username, -22} |\n| Rating: {player.CurrentRating, -22} |");
        }
        Console.WriteLine("|________________________________|");
    }

    public string GetInfo()
    {
        return "List of all players";
    }
}



public class PlayerStatisticsCommand : ICommand
{
    private readonly IAccountService _accountService;
    private readonly IGameService _gameService;

    public PlayerStatisticsCommand(IAccountService accountService, IGameService gameService)
    {
        _accountService = accountService;
        _gameService = gameService;
    }

    public void Execute()
    {
        Console.WriteLine("Enter the player's username or ID:");
        var input = Console.ReadLine()?.Trim();

        Account? player = null;

        if (int.TryParse(input, out var playerId))
        {
            player = _accountService.GetAccountById(playerId);
        }
        else
        {
            var allAccounts = _accountService.GetAllAccounts();
            player = allAccounts.FirstOrDefault(p => p.Username.Equals(input, StringComparison.OrdinalIgnoreCase));
        }

        if (player == null)
        {
            Console.WriteLine("Player not found.");
            return;
        }

        if (player is BonusAccount bonusAccount && player.Wins % 3 == 0)
        {
            player.CurrentRating += 100;
        }

        player.UpdateStatistics();

        Console.WriteLine(" ____________________________________");
        Console.WriteLine($"| Statistics for {player.Username,-19} |");
        Console.WriteLine("|------------------------------------|");
        Console.WriteLine($"| ID: {player.Id,-30} |");
        Console.WriteLine($"| Account Type: {player.AccountType,-20} |");
        Console.WriteLine($"| Current Rating: {player.CurrentRating,-18} |");
        Console.WriteLine($"| Games Played: {player.TotalGames,-20} |");
        Console.WriteLine($"| Wins: {player.Wins,-28} |");
        Console.WriteLine($"| Losses: {player.Losses,-26} |");
        Console.WriteLine("|____________________________________|");
        Console.WriteLine("\nGame History:");

//список ігор
        foreach (var game in player.PersonalGameHistory)
        {
            Console.WriteLine($"| {game.Player1.Username} {game.Result} against {game.Player2.Username} in {game.GameName}");
            Console.WriteLine($"| Rating Change: {game.RatingChange}\n");
        }
    }

    public string GetInfo()
    {
        return "Display the statistics of a specific player.";
    }
}




public class ListGamesCommand : ICommand
{
    private readonly IGameService _gameService;
    private IAccountService? accountService;

    public ListGamesCommand(IGameService gameService)
    {
        _gameService = gameService;
    }

    public ListGamesCommand(IGameService gameService, IAccountService accountService) : this(gameService)
    {
        this.accountService = accountService;
    }

    public void Execute()
    {
        var gameRecords = _gameService.GetAllGames();

        if (gameRecords == null || gameRecords.Count == 0)
        {
            Console.WriteLine("No games found.");
            return;
        }

        Console.WriteLine("List of all games:");
        foreach (var gameRecord in gameRecords)
        {
            Console.WriteLine($"| Game ID: {gameRecord.GameID}\n| Game Name: {gameRecord.GameName}\n| Player 1: {gameRecord.Player1.Username}\n| Player 2: {gameRecord.Player2.Username}\n| Result: {gameRecord.Result}\n| Rating Change: {gameRecord.RatingChange}");
        }
    }

    public string GetInfo()
    {
        return "List all games";
    }
}



public class ExitCommand : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Exit...");
        Environment.Exit(0);
    }

    public string GetInfo()
    {
        return "Program exit";
    }
}

public class Program
{
    public static void Main()
    {
        var dbContext = new DbContext();
        var accountRepository = new accountRepository(dbContext);
        var gameRecordRepository = new GameRecordRepository(dbContext);

        var accountService = new AccountService(accountRepository);
        var gameService = new GameService(gameRecordRepository);
        var commandProcessor = new CommandProcessor(accountService, gameService, accountRepository, gameRecordRepository, dbContext);
        commandProcessor.Run();
    }
}



public class CommandProcessor
{
    private readonly Dictionary<string, ICommand> _commands;

    public CommandProcessor(IAccountService accountService, IGameService gameService, IAccountRepository accountRepository, IGameRecordRepository gameRecordRepository,  DbContext dbContext)
    {
        var listPlayersCommand = new ListPlayersCommand(accountService);
        var listGamesCommand = new ListGamesCommand(gameService, accountService);

        _commands = new Dictionary<string, ICommand>
        {
            { "list players", listPlayersCommand },
            { "create player", new CreatePlayerCommand(accountService, listPlayersCommand, dbContext) },
            { "show games", new ListGamesCommand(gameService, accountService) },
            { "show stats", new PlayerStatisticsCommand(accountService, gameService) },
            { "create game", new CreateGameCommand(accountRepository, dbContext) },
            { "exit", new ExitCommand() },
        };
    }

    public void Run()
    {
        Console.WriteLine("\nType 'help' to view available commands or 'exit' to quit.");
        while (true)
        {
            Console.Write("\nEnter command: ");
            var input = Console.ReadLine()?.Trim()?.ToLower();

            if (string.IsNullOrEmpty(input)) continue;

            if (input == "help")
            {
                foreach (var kv in _commands)
                {
                    Console.WriteLine($"{kv.Key} - {kv.Value.GetInfo()}");
                }
                continue;
            }

            if (_commands.TryGetValue(input, out var command))
            {
                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing command: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Unknown command: '{input}'. Type 'help' for a list of commands.");
            }
        }
    }
}
