using ClientApi.Models;

namespace ClientApi.Services;

public interface IClientService
{
    Task<IEnumerable<ClientResponseDto>> GetAllClientsAsync();
    Task<ClientResponseDto> GetClientByIdAsync(int id);
    Task<ClientResponseDto> GetClientByEmailAsync(string email);
    Task<ClientResponseDto> AddClientAsync(ClientRequestDto clientDto);
    Task UpdateClientAsync(int id, ClientRequestDto clientDto);
    Task PartialUpdateClientAsync(int id, ClientPartialUpdateRequestDto clientDto);
    Task DeleteClientAsync(int id);
}