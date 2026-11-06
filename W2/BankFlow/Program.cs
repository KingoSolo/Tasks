using System.Globalization;

// Create application services once
AuthenticationServices authService = new();

IAccountFactory accountFactory = new AccountFactory();

BankAccountServices accountService =
    new BankAccountServices(accountFactory);

TransactionService transactionService =
    new TransactionService(accountService);

PrintResult printer = new();

// Read monetary amounts safely
decimal ReadAmount(string message)
{
    while (true)
    {
        Console.Write(message);

        string input = Console.ReadLine() ?? "";

        if (decimal.TryParse(
            input,
            NumberStyles.Number,
            CultureInfo.CurrentCulture,
            out decimal amount))
        {
            return amount;
        }

        Console.WriteLine("Invalid amount. Please enter a valid number.");
    }
}

// Read account number
string ReadAccountNumber(string message)
{
    Console.Write(message);
    return Console.ReadLine()?.Trim() ?? "";
}


while (true)
{
    Console.WriteLine("\n========== WELCOME TO BANKFLOW ==========");
    Console.WriteLine("1. Register");
    Console.WriteLine("2. Login");
    Console.WriteLine("3. Exit");

    Console.Write("Choose an option: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        // REGISTRATION
        case "1":

            string firstName;
            string lastName;

            while (true)
            {
                Console.Write("Enter your first name: ");
                firstName = Console.ReadLine() ?? "";

                Console.Write("Enter your last name: ");
                lastName = Console.ReadLine() ?? "";

                if (authService.IsValidName(firstName, lastName))
                {
                    break;
                }

                Console.WriteLine(
                    "Invalid names. Names cannot be blank or start with a digit.");
            }

            string email;

            while (true)
            {
                Console.Write("Enter your email: ");
                email = Console.ReadLine() ?? "";

                if (authService.IsValidEmail(email))
                {
                    break;
                }

                Console.WriteLine("Invalid email address. Try again.");
            }

            string password;

            while (true)
            {
                Console.Write("Enter your password: ");
                password = Console.ReadLine() ?? "";

                if (authService.IsValidPassword(password))
                {
                    break;
                }

                Console.WriteLine(
                    "Password must be at least 6 characters and contain a letter, a number and a special character.");
            }

            authService.SignUp(
                firstName,
                lastName,
                email,
                password);

            break;

        // LOGIN
        case "2":

            Console.Write("Enter your email: ");
            string loginEmail = Console.ReadLine() ?? "";

            Console.Write("Enter your password: ");
            string loginPassword = Console.ReadLine() ?? "";

            if (!authService.Login(loginEmail, loginPassword))
            {
                break;
            }

            bool loggedIn = true;

            // CUSTOMER BANKING MENU
            while (loggedIn)
            {
                Customer currentCustomer = authService.CurrentUser!;

                Console.WriteLine(
                    $"\n========== WELCOME {currentCustomer.FirstName.ToUpper()} ==========");

                Console.WriteLine("1. Create Account");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. Check Balance");
                Console.WriteLine("5. Transfer");
                Console.WriteLine("6. Account Details");
                Console.WriteLine("7. Statement of Account");
                Console.WriteLine("8. Logout");

                Console.Write("Choose an option: ");

                string? customerChoice = Console.ReadLine();

                switch (customerChoice)
                {
                    // CREATE ACCOUNT
                    case "1":

                        Console.WriteLine("\nSelect Account Type:");
                        Console.WriteLine("1. Savings");
                        Console.WriteLine("2. Current");

                        Console.Write("Choose account type: ");

                        string? accountChoice = Console.ReadLine();

                        AccountTypes accountType;

                        if (accountChoice == "1")
                        {
                            accountType = AccountTypes.Savings;
                        }
                        else if (accountChoice == "2")
                        {
                            accountType = AccountTypes.Current;
                        }
                        else
                        {
                            Console.WriteLine("Invalid account type.");
                            break;
                        }

                        accountService.CreateAccount(
                            currentCustomer,
                            accountType);

                        break;

                    // DEPOSIT
                    case "2":

                        if (currentCustomer.Accounts.Count == 0)
                        {
                            Console.WriteLine("You have no accounts. Create an account first.");
                            break;
                        }

                        string depositAccount = ReadAccountNumber(
                            "Enter account number: ");

                        decimal depositAmount = ReadAmount(
                            "Enter deposit amount: NGN ");

                        transactionService.Deposit(
                            currentCustomer,
                            depositAccount,
                            depositAmount);

                        break;

                    // WITHDRAW
                    case "3":

                        if (currentCustomer.Accounts.Count == 0)
                        {
                            Console.WriteLine("You have no accounts.");
                            break;
                        }

                        string withdrawalAccount = ReadAccountNumber(
                            "Enter account number: ");

                        decimal withdrawalAmount = ReadAmount(
                            "Enter withdrawal amount: NGN ");

                        transactionService.Withdraw(
                            currentCustomer,
                            withdrawalAccount,
                            withdrawalAmount);

                        break;

                    // CHECK BALANCE
                    case "4":

                        if (currentCustomer.Accounts.Count == 0)
                        {
                            Console.WriteLine("You have no accounts.");
                            break;
                        }

                        string balanceAccount = ReadAccountNumber(
                            "Enter account number: ");

                        accountService.GetBalance(
                            currentCustomer,
                            balanceAccount);

                        break;

                    // TRANSFER
                    case "5":

                        if (currentCustomer.Accounts.Count < 2)
                        {
                            Console.WriteLine(
                                "You need at least two accounts to transfer money.");
                            break;
                        }

                        string sourceAccount = ReadAccountNumber(
                            "Enter source account number: ");

                        string destinationAccount = ReadAccountNumber(
                            "Enter destination account number: ");

                        decimal transferAmount = ReadAmount(
                            "Enter transfer amount: NGN ");

                        transactionService.Transfer(
                            currentCustomer,
                            sourceAccount,
                            destinationAccount,
                            transferAmount);

                        break;

                    // ACCOUNT DETAILS
                    case "6":

                        printer.PrintAccountDetails(currentCustomer);

                        break;

                    // ACCOUNT STATEMENT
                    case "7":

                        if (currentCustomer.Accounts.Count == 0)
                        {
                            Console.WriteLine("You have no accounts.");
                            break;
                        }

                        string statementAccount = ReadAccountNumber(
                            "Enter account number: ");

                        Account? account = accountService.GetAccount(
                            currentCustomer,
                            statementAccount);

                        if (account == null)
                        {
                            Console.WriteLine("Account not found.");
                            break;
                        }

                        printer.PrintAccountStatement(account);

                        break;

                    // LOGOUT
                    case "8":

                        authService.Logout();
                        loggedIn = false;

                        break;

                    default:

                        Console.WriteLine("Invalid selection. Try again.");

                        break;
                }
            }

            break;

        // EXIT
        case "3":

            Console.WriteLine("Thank you for banking with BankFlow!");
            return;

        default:

            Console.WriteLine("Invalid selection. Try again.");

            break;
    }
}