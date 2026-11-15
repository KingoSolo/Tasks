using Microsoft.EntityFrameworkCore;
using WalletServiceAPI.Data;
using WalletServiceAPI.Models;


namespace WalletServiceAPI.Services;
public class WalletService : IWalletService
{
    private readonly ApplicationDbContext _context;

    public WalletService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet> GetOrCreateWalletAsync(string userId)
    {
        
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId);

        
        if (wallet != null)
        {
            return wallet;
        }

        wallet = new Wallet
        {
            UserId = userId,
            Balance = 0m
        };

        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();

        return wallet;
    }
}