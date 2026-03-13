using ClientApi.Models;

namespace ClientApi.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(string login, string password);
    Task LogoutAsync(string? refreshToken);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
}