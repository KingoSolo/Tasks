using WalletServiceAPI.Dto;

namespace WalletServiceAPI.Services;

public interface IWalletService
{
    Task<WalletResponseDto> GetOrCreateWalletAsync(string userId);
    Task FundWalletAsync(string userId, decimal amount);
    Task DebitWalletAsync(string userId, decimal amount);
    Task TransferAsync(string userId, string receiverAddress, decimal amount, string description);
    Task<List<TransactionResponseDto>> GetTransactionsAsync(string userId, int page, int pageSize);
}
