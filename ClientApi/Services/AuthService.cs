using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ClientApi.Models;
using ClientApi.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ClientApi.Services;

public class AuthService(IUserRepository userRepository, IConfiguration configuration, IPasswordHasher<User> passwordHasher, IRefreshTokenRepository refreshTokenRepository) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConfiguration _configuration = configuration;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<AuthResponseDto> LoginAsync(string login, string password)
    {
        var user = await _userRepository.GetByLoginAsync(login) ?? throw new KeyNotFoundException("User not found");

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.HashPassword, password);
        if (verificationResult == PasswordVerificationResult.Failed)
            return new AuthResponseDto { Success = false, Error = "Password is not verified" };

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays")),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };
        await _refreshTokenRepository.SaveRefreshTokenAsync(refreshTokenEntity);

        return new AuthResponseDto
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:AccessTokenExpirationMinutes"))
        };
    }

    public async Task LogoutAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken)) throw new BadHttpRequestException("Refresh Token is null or empty");

        await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken)) throw new BadHttpRequestException("Refresh Token is null or empty");

        var storedToken = await _refreshTokenRepository.GetValidRefreshTokenAsync(refreshToken);
        if (storedToken == null)
            return new AuthResponseDto { Success = false, Error = "Refresh Token is not found" };

        var user = await _userRepository.GetByIdAsync(storedToken.UserId);
        if (user == null)
            return new AuthResponseDto { Success = false, Error = "User not found" };

        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();

        await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken, newRefreshToken);

        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays")),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };
        await _refreshTokenRepository.SaveRefreshTokenAsync(newRefreshTokenEntity);

        return new AuthResponseDto
        {
            Success = true,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:AccessTokenExpirationMinutes"))
        };
    }

    private string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login)
        };

        var key = _configuration["Jwt:Key"]!;

        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:AccessTokenExpirationMinutes")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}