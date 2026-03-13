using System.ComponentModel.DataAnnotations;

namespace ClientApi.Models;

public class LoginRequest
{
    [Required(ErrorMessage = "Login is required"), StringLength(50, MinimumLength = 3, ErrorMessage = "Login must be between 3 and 50 characters long")]
    public string Login { get; set; } = null!;

    [Required(ErrorMessage = "Password is required"), StringLength(256, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 256 characters long")]
    public string Password { get; set; } = null!;
}

public class TokenRefreshRequest
{
    [Required(ErrorMessage = "RefreshToken is required")]
    public string RefreshToken { get; set; } = null!;
}

public class AuthResponseDto
{
    public bool Success { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Error { get; set; }
}