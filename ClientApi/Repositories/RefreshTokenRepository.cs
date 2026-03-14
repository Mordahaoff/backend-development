using ClientApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientApi.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<RefreshToken?> GetValidRefreshTokenAsync(string refreshToken)
    {
        Console.WriteLine($"Searching for token: {refreshToken}");
        var result = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken
                && rt.ExpiresAt > DateTime.UtcNow
                && !rt.IsRevoked);
        Console.WriteLine(result == null ? "Token not found or invalid" : $"Token found, expires at {result.ExpiresAt}");
        return result;
    }

    public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeRefreshTokenAsync(string token, string? replacedByToken = null)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            if (replacedByToken != null)
                refreshToken.ReplacedByToken = replacedByToken;

            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}