using ClientApi.Models;
using Microsoft.AspNetCore.Identity;

namespace ClientApi.Extensions;

public static class UserRequestDtoExtensions
{
    public static string HashPassword(this UserRequestDto userDto, IPasswordHasher<UserRequestDto> passwordHasher)
    {
        var login = userDto.Login;
        if (string.IsNullOrWhiteSpace(login)) throw new ArgumentException("Login musn't be null", nameof(userDto));

        var password = userDto.Password;
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password musn't be null", nameof(userDto));

        var hashPassword = passwordHasher.HashPassword(userDto, password);

        return hashPassword;
    }
}