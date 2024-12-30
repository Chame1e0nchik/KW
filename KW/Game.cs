using System.Security.Cryptography;
using System.Text;


public class CreateGameCommand : ICommand
{
    private readonly IAccountRepository _accountRepository;
    private readonly DbContext _dbContext;

    public CreateGameCommand(IAccountRepository accountRepository, DbContext dbContext)
    {
        _accountRepository = accountRepository;
        _dbContext = dbContext;
    }

    public void Execute()
    {
        // рєестрація гравців 1. тип гри 2. які гравці 3. виклик гри 4. зміни
        Console.WriteLine("Enter the game type (1 - Single, 2 - Training, 3 - Standard):");
        var gameTypeInput = Console.ReadLine()?.Trim();
// 1
        if (string.IsNullOrEmpty(gameTypeInput))
        {
            Console.WriteLine("Game type cannot be empty.");
            return;
        }

        GameType? gameType = gameTypeInput switch 
        {
            "1" => new SingleRatingGame(),
            "2" => new TrainingGame(),
            "3" => new StandardGame(),
            _ => null
        };

        if (gameType == null)
        {
            Console.WriteLine("Invalid game type selected.");
            return;
        }
//2
        var player1 = GetPlayer("Player 1");
        if (player1 == null) return;

        var player2 = GetPlayer("Player 2");
        if (player2 == null) return;

        if (player1.Id == player2.Id)
        {
            Console.WriteLine("Player 1 and Player 2 cannot be the same.");
            return;
        }
//3
        var winner = PlayTicTacToe(player1, player2);
        if (winner == null)
        {
            Console.WriteLine("Game ended in a draw.");
            return;
        }

        var result = winner == player1 ? GameType.GameResult.Win : GameType.GameResult.Lose;
        var ratingChange = gameType.PlayGame(player1, player2, result);

        var gameRecord = new GameRecord(_dbContext.GetNextGameId(), gameType.GameName, player1, player2, result.ToString(), ratingChange);
//4
        _dbContext.Games.Add(gameRecord);
        player1.PersonalGameHistory.Add(gameRecord);
        player2.PersonalGameHistory.Add(gameRecord);

        player1.UpdateStatistics();
        player2.UpdateStatistics();
        Console.WriteLine($"ID: {gameRecord.GameID}");
        Console.WriteLine($"Game successfully played: {gameType.GameName}");
        Console.WriteLine($"Winner: {winner.Username}");
        Console.WriteLine($"Rating change: {ratingChange}");
    }

// створення гри
    private Account? PlayTicTacToe(Account player1, Account player2)
    {
        
        Console.WriteLine($"Starting Tic-Tac-Toe between {player1.Username} (X) and {player2.Username} (O)");

        char[,] board = {
            { ' ', ' ', ' ' },
            { ' ', ' ', ' ' },
            { ' ', ' ', ' ' }
        };

        char currentPlayer = 'X';
        int turns = 0;

        while (turns < 9)
        {
            PrintBoard(board);
            Console.WriteLine($"Player {(currentPlayer == 'X' ? player1.Username : player2.Username)}, enter your move (row and column): ");
            string? input = Console.ReadLine();

            if (ParseInput(input, out int row, out int col) && PlaceMarker(board, row, col, currentPlayer))
            {
                turns++;
                if (CheckWin(board, currentPlayer))
                {
                    PrintBoard(board);
                    Console.WriteLine($"Player {(currentPlayer == 'X' ? player1.Username : player2.Username)} wins!");
                    return currentPlayer == 'X' ? player1 : player2;
                }
                currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
            }
            else
            {
                Console.WriteLine("Invalid move. Try again.");
            }
        }
        PrintBoard(board);
        Console.WriteLine("It's a draw!");
        return null;
    }
// кордони у масиві
    private void PrintBoard(char[,] board)
    {
        Console.WriteLine("\n {0} | {1} | {2} ", board[0, 0], board[0, 1], board[0, 2]);
        Console.WriteLine("---+---+---");
        Console.WriteLine(" {0} | {1} | {2} ", board[1, 0], board[1, 1], board[1, 2]);
        Console.WriteLine("---+---+---");
        Console.WriteLine(" {0} | {1} | {2} \n", board[2, 0], board[2, 1], board[2, 2]);
    }

    private bool ParseInput(string? input, out int row, out int col)
    {
        row = 0;
        col = 0;

        if (string.IsNullOrEmpty(input) || input.Length != 3 || input[1] != ' ')
        {
            return false;
        }

if (int.TryParse(input[0].ToString(), out row) && int.TryParse(input[2].ToString(), out col))
{
    row--; col--;

    bool isValidRow = row >= 0 && row < 3;
    bool isValidCol = col >= 0 && col < 3;

    return isValidRow && isValidCol;
}


        return false;
    }

    private bool PlaceMarker(char[,] board, int row, int col, char marker)
    {
        if (board[row, col] == ' ')
        {
            board[row, col] = marker;
            return true;
        }
        return false;
    }
// перевірка на перемогу
    private bool CheckWin(char[,] board, char marker)
    {
    for (int i = 0; i < 3; i++)
    {
        if (board[i, 0] == marker && board[i, 1] == marker && board[i, 2] == marker)
            return true;

        if (board[0, i] == marker && board[1, i] == marker && board[2, i] == marker)
            return true;
    }

    if (board[0, 0] == marker && board[1, 1] == marker && board[2, 2] == marker)
        return true;

    if (board[0, 2] == marker && board[1, 1] == marker && board[2, 0] == marker)
        return true;

    return false;
    }


    private Account? GetPlayer(string playerRole)
    {
    Console.WriteLine($"Enter {playerRole} ID:");
    if (!int.TryParse(Console.ReadLine(), out var playerId))
    {
        Console.WriteLine($"Invalid ID for {playerRole}.");
        return null;
    }

    try
    {
    var player = _accountRepository.ReadById(playerId);

    if (player == null)
    {
        Console.WriteLine($"{playerRole} does not exist.");
        return null;
    }

    if (string.IsNullOrEmpty(player.Password))
    {
        Console.WriteLine($"{playerRole} {player.Username} has no password set.");
        return player;
    }

    Console.WriteLine($"Enter password for {playerRole} ({player.Username}):");
    var password = Console.ReadLine()?.Trim();
    if (
        password == null
        )
{
    Console.WriteLine("Password cannot be null.");
    return null;
}
    if (
        HashPassword(password) != player.Password
        )
    {
        Console.WriteLine("Incorrect password. Access denied.");
        return null;
    }

    return player;
    }

    catch (Exception ex)
    {
        Console.WriteLine($"Error retrieving {playerRole}: {ex.Message}");
        return null;
    }

    }

    private string HashPassword(string password)
    {
    {
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes); // хеш замість пароля
        }
    }
    }

    public string GetInfo()
    {
        return "Create a new game.";
    }
}


