public class AccountStatement
{
    public DateTime Date { get; set; }

    public required string Description { get; set; }

    public required string TransactionType { get; set; }

    public decimal Amount { get; set; }

    public decimal Balance { get; set; }
}