
public class Account
{
    public AccountTypes Type{get; set;}
    public required string AccountNumber{get; set;}
    public decimal Balance{get; set;}
    public List<AccountStatement> History{get; set;} = new ();

}