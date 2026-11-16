using Microsoft.EntityFrameworkCore;
using WalletServiceAPI.Data;
using WalletServiceAPI.Dto;
using WalletServiceAPI.Models;

namespace WalletServiceAPI.Services;

public class WalletService : IWalletService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WalletService> _logger;

    public WalletService(ApplicationDbContext context,ILogger<WalletService> logger)
    {
        _context = context;
        _logger = logger;
    }

    private async Task<Wallet> GetOrCreateWalletEntityAsync(string userId)
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

        _logger.LogInformation(
            "Created new wallet {WalletAddress} with default currency {Currency} for user {UserId}",
            wallet.WalletAddress, wallet.Currency, userId);


        return wallet;
    }

    public async Task<WalletResponseDto> GetOrCreateWalletAsync(string userId)
    {
        var wallet = await GetOrCreateWalletEntityAsync(userId);

        return new WalletResponseDto
        {
            WalletId = wallet.WalletId,
            WalletAddress = wallet.WalletAddress,
            Balance = wallet.Balance,
            Currency = wallet.Currency,
            CreatedAt = wallet.CreatedAt
        };
    }

    public async Task FundWalletAsync(string userId, decimal amount)
    {
        if (amount <= 0){
            _logger.LogWarning(
                    "Invalid funding amount {Amount} attempted by user {UserId}",
                    amount, userId);
            throw new ArgumentException("Amount must be greater than zero.");
        }
        var wallet = await GetOrCreateWalletEntityAsync(userId);

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
         _logger.LogInformation(
            "Wallet {WalletAddress} funded with {Amount} {Currency}. New balance: {Balance}",
            wallet.WalletAddress, amount, wallet.Currency, wallet.Balance);
    }

    public async Task DebitWalletAsync(string userId, decimal amount)
    {
        _logger.LogInformation(
            "Wallet debit started for user {UserId} with amount {Amount}",
            userId, amount);

        if (amount <= 0)
        {
            _logger.LogWarning(
                "Invalid debit amount {Amount} attempted by user {UserId}",
                amount, userId);
            throw new ArgumentException("Amount must be greater than zero.");
        }
        var wallet = await GetOrCreateWalletEntityAsync(userId);

        if (wallet.Balance < amount)
        {
             _logger.LogWarning(
                "Debit failed for user {UserId}: insufficient balance ({Balance} < {Amount})",
                userId, wallet.Balance, amount);
            throw new InvalidOperationException("Insufficient wallet balance.");
        }
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
        _logger.LogInformation(
            "Wallet {WalletAddress} debited by {Amount} {Currency}. New balance: {Balance}",
            wallet.WalletAddress, amount, wallet.Currency, wallet.Balance);
    }

   public async Task TransferAsync(
        string userId,
        string receiverAddress,
        decimal amount,
        string description)
    {
        _logger.LogInformation(
            "Transfer initiated by user {UserId} to wallet {ReceiverAddress} for amount {Amount}",
            userId, receiverAddress, amount);

        if (amount <= 0)
        {
            _logger.LogWarning(
                "Invalid transfer amount {Amount} attempted by user {UserId}",
                amount, userId);
            throw new ArgumentException("Amount must be greater than zero.");
        }

        var sender = await GetOrCreateWalletEntityAsync(userId);

        var receiver = await _context.Wallets
            .FirstOrDefaultAsync(w => w.WalletAddress == receiverAddress);

        if (receiver == null)
        {
            _logger.LogWarning(
                "Transfer failed: receiver wallet {ReceiverAddress} not found",
                receiverAddress);
            throw new KeyNotFoundException("Receiver wallet not found.");
        }

        if (sender.WalletId == receiver.WalletId)
        {
            _logger.LogWarning(
                "Transfer failed: user {UserId} attempted self-transfer to {WalletAddress}",
                userId, receiverAddress);
            throw new InvalidOperationException("Cannot transfer to your own wallet.");
        }

        if (sender.Currency != receiver.Currency)
        {
            _logger.LogWarning(
                "Transfer failed: currency mismatch ({SenderCurrency} vs {ReceiverCurrency})",
                sender.Currency, receiver.Currency);
            throw new InvalidOperationException("Wallet currencies do not match.");
        }

        if (sender.Balance < amount)
        {
            _logger.LogWarning(
                "Transfer failed for user {UserId}: insufficient balance ({Balance} < {Amount})",
                userId, sender.Balance, amount);
            throw new InvalidOperationException("Insufficient wallet balance.");
        }

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

            _logger.LogInformation(
                "Transfer {ReferenceId} completed: {Amount} {Currency} from {SenderAddress} to {ReceiverAddress}",
                reference, amount, sender.Currency, sender.WalletAddress, receiver.WalletAddress);
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync();
            _logger.LogError(
                ex,
                "Transfer failed and rolled back for user {UserId} to {ReceiverAddress}",
                userId, receiverAddress);
            throw;
        }
    }

    public async Task<List<TransactionResponseDto>> GetTransactionsAsync(
        string userId, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        _logger.LogInformation(
            "Fetching transaction history for user {UserId} (Page {Page}, PageSize {PageSize})",
            userId, page, pageSize);

        var transactions = await _context.Transactions
            .Where(t => t.Wallet.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return transactions.Select(t => new TransactionResponseDto
        {
            TransactionId = t.TransactionId,
            Amount = t.Amount,
            TransactionType = t.TransactionType,
            ReferenceId = t.ReferenceId,
            Description = t.Description,
            TransactionStatus = t.TransactionStatus,
            CreatedAt = t.CreatedAt
        }).ToList();
    }
}