
public class PrintResult
{
    // Print all customer accounts
    public void PrintAccountDetails(Customer customer)
    {
        Console.WriteLine("\n================ ACCOUNT DETAILS ================\n");

        Console.WriteLine(
            "{0,-20} {1,-20} {2,-15} {3,15}",
            "FULL NAME",
            "ACCOUNT NUMBER",
            "ACCOUNT TYPE",
            "AMOUNT BAL");

        Console.WriteLine(
            new string('-', 75));

        if (customer.Accounts.Count == 0)
        {
            Console.WriteLine("No accounts found.");
            return;
        }

        foreach (Account account in customer.Accounts)
        {
            Console.WriteLine(
                "{0,-20} {1,-20} {2,-15} {3,15:N2}",
                $"{customer.FirstName} {customer.LastName}",
                account.AccountNumber,
                account.Type,
                account.Balance);
        }

        Console.WriteLine(new string('-', 75));
    }

    // Print account statement
    public void PrintAccountStatement(Account account)
    {
        Console.WriteLine(
            $"\n========== ACCOUNT STATEMENT: {account.AccountNumber} ==========\n");

        Console.WriteLine(
            "{0,-20} {1,-35} {2,15} {3,15}",
            "DATE",
            "DESCRIPTION",
            "AMOUNT",
            "BALANCE");

        Console.WriteLine(new string('-', 90));

        if (account.History.Count == 0)
        {
            Console.WriteLine("No transactions found.");
            return;
        }

        foreach (AccountStatement statement in account.History
                     .OrderByDescending(s => s.Date))
        {
            Console.WriteLine(
                "{0,-20} {1,-35} {2,15:N2} {3,15:N2}",
                statement.Date.ToString("dd/MM/yyyy HH:mm"),
                statement.Description,
                statement.Amount,
                statement.Balance);
        }

        Console.WriteLine(new string('-', 90));
    }
}