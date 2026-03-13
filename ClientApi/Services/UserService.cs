using ClientApi.Extensions;
using ClientApi.Models;
using ClientApi.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;

namespace ClientApi.Services;

public class UserService(IUserRepository userRepository, IPasswordHasher<UserRequestDto> passwordHasher) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher<UserRequestDto> _passwordHasher = passwordHasher;

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            Login = u.Login,
        });
    }

    public async Task<UserResponseDto> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");

        return new UserResponseDto { Id = user.Id, Login = user.Login };
    }

    public async Task<UserResponseDto> GetUserByLoginAsync(string login)
    {
        if (string.IsNullOrEmpty(login))
            throw new BadHttpRequestException("Login is null or empty");

        var user = await _userRepository.GetByLoginAsync(login) ?? throw new ArgumentException("User not found", nameof(login));

        return new UserResponseDto { Id = user.Id, Login = user.Login };
    }

    public async Task<UserResponseDto> AddUserAsync(UserRequestDto userDto)
    {
        var existing = await _userRepository.GetByLoginAsync(userDto.Login);
        if (existing != null)
            throw new InvalidOperationException("User with this login already exists");

        var hashedPassword = UserRequestDtoExtensions.HashPassword(userDto, _passwordHasher);

        var user = new User
        {
            Login = userDto.Login,
            HashPassword = hashedPassword
        };

        var userDb = await _userRepository.AddAsync(user);

        var responseDto = new UserResponseDto { Id = userDb.Id, Login = userDb.Login };
        return responseDto;
    }

    public async Task UpdateUserAsync(int id, UserRequestDto userDto)
    {
        var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");

        var existing = await _userRepository.GetByLoginAsync(userDto.Login);
        if (existing != null)
            throw new InvalidOperationException("User with this login already exists");

        var hashedPassword = UserRequestDtoExtensions.HashPassword(userDto, _passwordHasher);

        user.Login = userDto.Login;
        user.HashPassword = hashedPassword;

        await _userRepository.UpdateAsync(user);
    }

    public async Task PartialUpdateUserAsync(int id, UserPartialUpdateRequestDto userDto)
    {
        var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");

        var patchDoc = new JsonPatchDocument<User>();
        if (userDto.Login != null)
        {
            var existing = await _userRepository.GetByLoginAsync(userDto.Login);
            if (existing != null)
                throw new InvalidOperationException("User with this login already exists");
            patchDoc.Replace(u => u.Login, userDto.Login);
        }
        if (userDto.Password != null)
        {
            var hashedPassword = UserRequestDtoExtensions.HashPassword(userDto, _passwordHasher);
            patchDoc.Replace(u => u.HashPassword, hashedPassword);
        }

        if (patchDoc.Operations.Count == 0)
            throw new BadHttpRequestException("No fields to update.");

        patchDoc.ApplyTo(user);

        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        _ = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");

        await _userRepository.DeleteAsync(id);
    }
}