using WalletServiceAPI.Models;
namespace WalletServiceAPI.Services;
public interface IWalletService
{
    Task<Wallet> GetOrCreateWalletAsync(string userId);
}