public class TransactionService
{
    private readonly BankAccountServices _accountService;

    private const decimal SavingsMinimumBalance = 1000m;

    public TransactionService(BankAccountServices accountService)
    {
        _accountService = accountService;
    }

    // Record transaction
    private void RecordTransaction(
        Account account,
        string description,
        string transactionType,
        decimal amount)
    {
        AccountStatement statement = new AccountStatement
        {
            Date = DateTime.Now,
            Description = description,
            TransactionType = transactionType,
            Amount = amount,
            Balance = account.Balance
        };

        account.History.Add(statement);
    }

    // Deposit
    public bool Deposit(
        Customer customer,
        string accountNumber,
        decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Deposit amount must be greater than zero.");
            return false;
        }

        Account? account =
            _accountService.GetAccount(customer, accountNumber);

        if (account == null)
        {
            Console.WriteLine("Account not found.");
            return false;
        }

        account.Balance += amount;

        RecordTransaction(
            account,
            "Cash deposit",
            "Deposit",
            amount);

        Console.WriteLine("Deposit successful!");
        Console.WriteLine($"New Balance: NGN {account.Balance:N2}");

        return true;
    }

    // Withdraw
    public bool Withdraw(
        Customer customer,
        string accountNumber,
        decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Withdrawal amount must be greater than zero.");
            return false;
        }

        Account? account =
            _accountService.GetAccount(customer, accountNumber);

        if (account == null)
        {
            Console.WriteLine("Account not found.");
            return false;
        }

        if (amount > account.Balance)
        {
            Console.WriteLine("Insufficient funds.");
            return false;
        }

        if (account.Type == AccountTypes.Savings &&
            account.Balance - amount < SavingsMinimumBalance)
        {
            Console.WriteLine(
                "Savings accounts must maintain a minimum balance of NGN 1,000.");
            return false;
        }

        account.Balance -= amount;

        RecordTransaction(
            account,
            "Cash withdrawal",
            "Withdrawal",
            amount);

        Console.WriteLine("Withdrawal successful!");
        Console.WriteLine($"New Balance: NGN {account.Balance:N2}");

        return true;
    }

    
    public bool Transfer(
        Customer customer,
        string sourceAccountNumber,
        string destinationAccountNumber,
        decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Transfer amount must be greater than zero.");
            return false;
        }

        if (sourceAccountNumber == destinationAccountNumber)
        {
            Console.WriteLine("You cannot transfer to the same account.");
            return false;
        }

        Account? sourceAccount =
            _accountService.GetAccount(customer, sourceAccountNumber);

        Account? destinationAccount =
            _accountService.GetAccount(customer, destinationAccountNumber);

        if (sourceAccount == null || destinationAccount == null)
        {
            Console.WriteLine(
                "One or both accounts could not be found.");
            return false;
        }

        if (amount > sourceAccount.Balance)
        {
            Console.WriteLine("Insufficient funds.");
            return false;
        }

        if (sourceAccount.Type == AccountTypes.Savings &&
            sourceAccount.Balance - amount < SavingsMinimumBalance)
        {
            Console.WriteLine(
                "Savings accounts must maintain a minimum balance of NGN 1,000.");
            return false;
        }

    
        sourceAccount.Balance -= amount;
        destinationAccount.Balance += amount;

        RecordTransaction(
            sourceAccount,
            $"Transfer to {destinationAccount.AccountNumber}",
            "Transfer Debit",
            amount);

        RecordTransaction(
            destinationAccount,
            $"Transfer from {sourceAccount.AccountNumber}",
            "Transfer Credit",
            amount);

        Console.WriteLine("Transfer successful!");

        return true;
    }
}