using ClientApi.Models;

namespace ClientApi.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetValidRefreshTokenAsync(string refreshToken);
    Task SaveRefreshTokenAsync(RefreshToken refreshToken);
    Task RevokeRefreshTokenAsync(string token, string? replacedByToken = null);
}