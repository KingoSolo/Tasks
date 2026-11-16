using ToDo.DTOs;

namespace ToDo.Services;

public interface IAuthService
{
    Task<(bool Succeeded, string Message, IEnumerable<string>? Errors)> RegisterAsync(RegisterDto request);
    Task<(bool Succeeded, string Message, string? Token, DateTime? Expiration)> LoginAsync(LoginDto request);
}
