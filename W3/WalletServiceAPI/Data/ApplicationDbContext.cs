using Microsoft.EntityFrameworkCore;
using WalletServiceAPI.Models;

namespace WalletServiceAPI.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Wallet> Wallets {get; set;} = null!; 
    public DbSet<Transaction> Transactions {get; set;} = null!;
}