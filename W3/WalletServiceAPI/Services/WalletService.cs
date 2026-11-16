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
            WalletAddress = Guid.NewGuid().ToString("N"),
            Balance = 0m
        };

        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();

        return wallet;
    }
    public async Task FundWalletAsync(string userId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        var wallet = await GetOrCreateWalletAsync(userId);

        wallet.Balance += amount;

        _context.Transactions.Add(new Transaction
        {
            WalletId = wallet.WalletId,
            Amount = amount,
            TransactionType = "Credit",
            ReferenceId = Guid.NewGuid().ToString(),
            Description = "Wallet funding",
            TransactionStatus = "Completed",
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task DebitWalletAsync(string userId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        var wallet = await GetOrCreateWalletAsync(userId);

        if (wallet.Balance < amount)
            throw new InvalidOperationException("Insufficient wallet balance.");

        wallet.Balance -= amount;

        _context.Transactions.Add(new Transaction
        {
            WalletId = wallet.WalletId,
            Amount = amount,
            TransactionType = "Debit",
            ReferenceId = Guid.NewGuid().ToString(),
            Description = "Wallet debit",
            TransactionStatus = "Completed",
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task TransferAsync(
        string userId,
        string receiverAddress,
        decimal amount,
        string description)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        var sender = await GetOrCreateWalletAsync(userId);

        var receiver = await _context.Wallets
            .FirstOrDefaultAsync(w => w.WalletAddress == receiverAddress);

        if (receiver == null)
            throw new KeyNotFoundException("Receiver wallet not found.");

        if (sender.WalletId == receiver.WalletId)
            throw new InvalidOperationException("Cannot transfer to your own wallet.");

        if (sender.Currency != receiver.Currency)
            throw new InvalidOperationException("Wallet currencies do not match.");

        if (sender.Balance < amount)
            throw new InvalidOperationException("Insufficient wallet balance.");

        await using var dbTransaction = await _context.Database.BeginTransactionAsync();

        try
        {
            sender.Balance -= amount;
            receiver.Balance += amount;

            var reference = Guid.NewGuid().ToString();

            _context.Transactions.AddRange(
                new Transaction
                {
                    WalletId = sender.WalletId,
                    Amount = amount,
                    TransactionType = "TransferDebit",
                    ReferenceId = reference,
                    Description = description,
                    TransactionStatus = "Completed",
                    CreatedAt = DateTime.UtcNow
                },
                new Transaction
                {
                    WalletId = receiver.WalletId,
                    Amount = amount,
                    TransactionType = "TransferCredit",
                    ReferenceId = reference,
                    Description = description,
                    TransactionStatus = "Completed",
                    CreatedAt = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Transaction>> GetTransactionsAsync(
        string userId, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        return await _context.Transactions
            .Where(t => t.Wallet.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}