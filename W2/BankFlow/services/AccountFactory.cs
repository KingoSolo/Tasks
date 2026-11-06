public interface IAccountFactory
{
    Account CreateAccount(AccountTypes accountType);
}

public class AccountFactory : IAccountFactory
{
    private static readonly HashSet<string> _generatedNumbers = new();

    public Account CreateAccount(AccountTypes accountType)
    {
        string accountNumber;

        do
        {
            accountNumber = Random.Shared
                .NextInt64(1_000_000_000, 10_000_000_000)
                .ToString();

        } while (!_generatedNumbers.Add(accountNumber));

        return new Account
        {
            AccountNumber = accountNumber,
            Type = accountType,
            Balance = 0m
        };
    }
}