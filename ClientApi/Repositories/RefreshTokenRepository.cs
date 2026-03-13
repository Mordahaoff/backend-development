using ClientApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientApi.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<RefreshToken?> GetValidRefreshTokenAsync(string refreshToken)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken
                && rt.ExpiresAt > DateTime.UtcNow
                && !rt.IsRevoked);
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

            await _context.SaveChangesAsync();
        }
    }
}