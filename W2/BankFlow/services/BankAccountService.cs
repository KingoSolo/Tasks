public class BankAccountServices
{
    private readonly IAccountFactory _accountFactory;
    private readonly List<Customer> _allCustomers;

    public BankAccountServices(IAccountFactory accountFactory, List<Customer> allCustomers)
    {
        _accountFactory = accountFactory;
        _allCustomers = allCustomers;
    }

    // Create a new account
    public Account CreateAccount(Customer customer, AccountTypes type)
    {
        Account newAccount = _accountFactory.CreateAccount(type);

        customer.Accounts.Add(newAccount);

        Console.WriteLine("\nAccount created successfully!");
        Console.WriteLine($"Account Number: {newAccount.AccountNumber}");
        Console.WriteLine($"Account Type: {newAccount.Type}");

        return newAccount;
    }

    // Find an account belonging to the customer
    public Account? GetAccount(Customer customer, string accountNumber)
    {
        return customer.Accounts.FirstOrDefault(
            account => account.AccountNumber == accountNumber);
    }

    // Get account balance
    public bool GetBalance(Customer customer, string accountNumber)
    {
        Account? account = GetAccount(customer, accountNumber);

        if (account == null)
        {
            Console.WriteLine("Account not found.");
            return false;
        }

        Console.WriteLine(
            $"Account Balance: NGN {account.Balance:N2}");

        return true;
    }

    //Search for a customers account number 
    public Account? GetAccountByNumber(string accountNumber)
    {
        return _allCustomers
            .SelectMany(c => c.Accounts)
            .FirstOrDefault(a => a.AccountNumber == accountNumber);
    }
}