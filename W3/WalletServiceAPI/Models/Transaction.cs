namespace WalletServiceAPI.Models;
public class Transaction
{
    public int TransactionId{get;set;} 
    public int WalletId{get;set;}
    public decimal Amount{get;set;}
    public string TransactionType{get;set;} = "";
    public string ReferenceId{get;set;} = "";
    public string Description{get;set;} = "";
    public string TransactionStatus{get;set;} = "";
    public DateTime CreatedAt{get;set;} = DateTime.UtcNow;
    public Wallet Wallet{get;set;} = null!;
}