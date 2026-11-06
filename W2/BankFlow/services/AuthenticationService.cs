using System.Net.Mail;

public class AuthenticationServices
{
    public Customer? CurrentUser { get; private set; }

    private readonly List<Customer> _allCustomers = new();

    // Validate names
    public bool IsValidName(string firstName, string lastName)
    {
        firstName = firstName.Trim();
        lastName = lastName.Trim();

        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName))
        {
            return false;
        }

        return char.IsLetter(firstName[0]) &&
               char.IsLetter(lastName[0]);
    }

    // Sanitize names
    public (string FirstName, string LastName) SanitizeNames(
        string firstName,
        string lastName)
    {
        firstName = firstName.Trim();
        lastName = lastName.Trim();

        firstName =
            char.ToUpperInvariant(firstName[0]) + firstName[1..];

        lastName =
            char.ToUpperInvariant(lastName[0]) + lastName[1..];

        return (firstName, lastName);
    }

    // Validate password
    public bool IsValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 6)
        {
            return false;
        }

        bool hasLetter = password.Any(char.IsLetter);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecialCharacter =
            password.Any(c => "@#$%^&!".Contains(c));

        return hasLetter && hasDigit && hasSpecialCharacter;
    }

    // Validate email
    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var validEmail = new MailAddress(email.Trim());

            return validEmail.Address == email.Trim();
        }
        catch (FormatException)
        {
            return false;
        }
    }

    // Register customer
    public bool SignUp(
        string firstName,
        string lastName,
        string email,
        string password)
    {
        if (!IsValidName(firstName, lastName))
        {
            Console.WriteLine("Invalid customer name.");
            return false;
        }

        if (!IsValidEmail(email))
        {
            Console.WriteLine("Invalid email address.");
            return false;
        }

        if (!IsValidPassword(password))
        {
            Console.WriteLine(
                "Password must contain at least 6 characters, a letter, a number and a special character.");
            return false;
        }

        email = email.Trim();

        bool emailExists = _allCustomers.Any(
            customer => customer.Email.Equals(
                email,
                StringComparison.OrdinalIgnoreCase));

        if (emailExists)
        {
            Console.WriteLine("This email is already registered.");
            return false;
        }

        var sanitizedNames = SanitizeNames(firstName, lastName);

        Customer newCustomer = new Customer
        {
            FirstName = sanitizedNames.FirstName,
            LastName = sanitizedNames.LastName,
            Email = email,
            Password = password
        };

        _allCustomers.Add(newCustomer);

        Console.WriteLine("Registration successful!");

        return true;
    }

    // Login
    public bool Login(string enteredEmail, string enteredPassword)
    {
        if (string.IsNullOrWhiteSpace(enteredEmail) ||
            string.IsNullOrEmpty(enteredPassword))
        {
            Console.WriteLine("Email and password are required.");
            return false;
        }

        Customer? customer = _allCustomers.FirstOrDefault(
            c => c.Email.Equals(
                enteredEmail.Trim(),
                StringComparison.OrdinalIgnoreCase)
                && c.Password == enteredPassword);

        if (customer == null)
        {
            Console.WriteLine("Invalid email or password.");
            return false;
        }

        CurrentUser = customer;

        Console.WriteLine(
            $"Welcome, {CurrentUser.FirstName} {CurrentUser.LastName}!");

        return true;
    }

    // Logout
    public void Logout()
    {
        if (CurrentUser == null)
        {
            Console.WriteLine("No customer is currently logged in.");
            return;
        }

        CurrentUser = null;

        Console.WriteLine("Logged out successfully.");
    }
}