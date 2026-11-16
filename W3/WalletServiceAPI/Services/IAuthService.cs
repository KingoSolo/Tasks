using WalletServiceAPI.Dto;

namespace WalletServiceAPI.Services;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);
    Task<string?> LoginAsync(LoginDto dto);
}
