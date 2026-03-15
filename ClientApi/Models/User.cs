using System.ComponentModel.DataAnnotations;

namespace ClientApi.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = null!;
    public string HashPassword { get; set; } = null!;
}

public class UserRequestDto
{
    [Required(ErrorMessage = "Login is required"), StringLength(50, MinimumLength = 3, ErrorMessage = "Login must be between 3 and 50 characters long")]
    public string Login { get; set; } = null!;

    [Required(ErrorMessage = "Password is required"), StringLength(256, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 256 characters long")]
    public string Password { get; set; } = null!;
}

public class UserPartialUpdateRequestDto : UserRequestDto
{
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Login must be between 3 and 50 characters long")]
    public new string? Login { get; set; }

    [StringLength(256, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 256 characters long")]
    public new string? Password { get; set; }
}

public class UserResponseDto
{
    public int Id { get; set; }
    public string Login { get; set; } = null!;
}