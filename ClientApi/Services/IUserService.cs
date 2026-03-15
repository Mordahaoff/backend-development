using ClientApi.Models;

namespace ClientApi.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto?>> GetAllUsersAsync();
    Task<UserResponseDto> GetUserByIdAsync(int id);
    Task<UserResponseDto> GetUserByLoginAsync(string login);
    Task<UserResponseDto> AddUserAsync(UserRequestDto userDto);
    Task UpdateUserAsync(int id, UserRequestDto userDto);
    Task PartialUpdateUserAsync(int id, UserPartialUpdateRequestDto userDto);
    Task DeleteUserAsync(int id);
}