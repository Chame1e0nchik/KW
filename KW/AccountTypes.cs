public class AccountFactory
{
    public static Account CreateAccount(string accountType, string username)
    {
        return accountType switch
        {
            "1" => new RegularAccount(username),
            "2" => new BonusAccount(username),
            "3" => new HardcoreAccount(username),
            _ => throw new ArgumentException("Invalid account type.")
        };
    }
}


public class RegularAccount : Account
{
    public RegularAccount(string username)
    {
        RatingMultiplier = 1;
        Level = AccountLevel.Standard;
        Username = username;
        CurrentRating = 1000;
    }
    public override string AccountType => "Regular Account";
}

public class BonusAccount : Account
{
    public BonusAccount(string username)
    {
        RatingMultiplier = 1;
        Level = AccountLevel.Easy;
        Username = username;
        CurrentRating = 1000;
        WinsInRow = 0;
    }
    public override string AccountType => "Bonus Account";
}

public class HardcoreAccount : Account
{
    public HardcoreAccount(string username)
    {
        RatingMultiplier = 2;
        Level = AccountLevel.Hardcore;
        Username = username;
        CurrentRating = 1500;
    }
    public override string AccountType => "Hardcore Account";
}