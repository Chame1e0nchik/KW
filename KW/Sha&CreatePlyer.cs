using System.Security.Cryptography;
using System.Text;

public class CreatePlayerCommand : ICommand
{
    private readonly IAccountService _accountService;
    private readonly ICommand _listPlayersCommand;
    private readonly DbContext _dbContext;

    public CreatePlayerCommand(IAccountService accountService, ICommand listPlayersCommand, DbContext dbContext)
    {
        _accountService = accountService;
        _listPlayersCommand = listPlayersCommand;
        _dbContext = dbContext;
    }

    public void Execute()
    {
        Console.WriteLine("Enter the username for the new player:");
        var username = Console.ReadLine()?.Trim();

        /*перевірки*/
        if (string.IsNullOrEmpty(username))
        {
            Console.WriteLine("Username cannot be empty.");
            return;
        }

        if (username.Length < 3 || username.Length > 10)
        {
            Console.WriteLine("Username must be between 3 and 10 characters long.");
            return;
        }

        if (!username.All(char.IsLetterOrDigit))
        {
            Console.WriteLine("Username can only contain letters and digits.");
            return;
        }
        /*перевірки*/

        Console.WriteLine("Select account type:");
        Console.WriteLine("1 - Regular Account");
        Console.WriteLine("2 - Bonus Account");
        Console.WriteLine("3 - Hardcore Account");
        var accountTypeInput = Console.ReadLine()?.Trim();

        Account? newPlayer;
        try
        {
            newPlayer = AccountFactory.CreateAccount(accountTypeInput, username);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        if (newPlayer == null)
        {
            Console.WriteLine("Invalid account type selected. Please try again.");
            return;
        }

        // пароль
        Console.WriteLine("Set a password for the new player:");
        var password = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        if (password.Length < 8)
        {
            Console.WriteLine("Password must be at least 8 characters long.");
            return;
        }

        // хеш замість пароля
        var hashedPassword = HashPassword(password);
        var newPlayerId = _dbContext.GetNextPlayerId();
        newPlayer.Id = newPlayerId;
        newPlayer.CurrentRating = 0;
        newPlayer.Password = hashedPassword; // зберігання хешу замість пароля

        _accountService.CreateAccount(newPlayer);

        Console.WriteLine($"Player {username} created successfully as {newPlayer.GetType().Name} with ID {newPlayerId}, rating {newPlayer.CurrentRating}, and password set.");
        Console.WriteLine("\nUpdated list of players:");
        _listPlayersCommand.Execute();
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes); // хеш замість пароля
        }
    }

    public string GetInfo()
    {
        return "Create a new player";
    }
    
}
