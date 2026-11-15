namespace WalletServiceAPI.Models;
public class Wallet
{
    public int WalletId {get;set;}
    public string UserId{get;set;} = "";
    public string WalletAddress{get;set;} = "";
    public decimal Balance{get;set;}
    public string Currency{get;set;} = "NGN";
    public DateTime CreatedAt{get;set;} = DateTime.UtcNow;
    public ICollection<Transaction> Transactions{get;set;}  = new List<Transaction>();
    }