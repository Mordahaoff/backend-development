using ClientApi.Models;
using ClientApi.Repositories;
using Microsoft.AspNetCore.JsonPatch;

namespace ClientApi.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<IEnumerable<ClientResponseDto>> GetAllClientsAsync()
    {
        var clients = await _clientRepository.GetAllAsync();

        return clients.Select(c => new ClientResponseDto
        {
            Id = c.Id,
            FullName = c.FullName,
            Phone = c.Phone,
            Email = c.Email,
            Discount = c.Discount,
            Verified = c.Verified
        });
    }

    public async Task<ClientResponseDto> GetClientByIdAsync(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Client not found");

        return new ClientResponseDto
        {
            Id = client.Id,
            FullName = client.FullName,
            Phone = client.Phone,
            Email = client.Email,
            Discount = client.Discount,
            Verified = client.Verified
        };
    }

    public async Task<ClientResponseDto> GetClientByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new BadHttpRequestException("Email is null or empty");

        var client = await _clientRepository.GetByEmailAsync(email)
            ?? throw new KeyNotFoundException("Client not found");

        return new ClientResponseDto
        {
            Id = client.Id,
            FullName = client.FullName,
            Phone = client.Phone,
            Email = client.Email,
            Discount = client.Discount,
            Verified = client.Verified
        };
    }

    public async Task<ClientResponseDto> AddClientAsync(ClientRequestDto clientDto)
    {
        var existing = _clientRepository.GetByEmailAsync(clientDto.Email);
        if (existing != null)
            throw new InvalidOperationException("Client with this email already exists");

        var client = new Client
        {
            FullName = clientDto.FullName,
            Phone = clientDto.Phone,
            Email = clientDto.Email,
            Discount = clientDto.Discount,
            Verified = clientDto.Verified
        };

        var clientDb = await _clientRepository.AddAsync(client);

        var responseDto = new ClientResponseDto
        {
            Id = clientDb.Id,
            FullName = clientDb.FullName,
            Phone = clientDb.Phone,
            Email = clientDb.Email,
            Discount = clientDb.Discount,
            Verified = clientDb.Verified
        };
        return responseDto;
    }

    public async Task UpdateClientAsync(int id, ClientRequestDto clientDto)
    {
        var client = await _clientRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Client not found");

        var existing = _clientRepository.GetByEmailAsync(clientDto.Email);
        if (existing != null)
            throw new InvalidOperationException("Client with this email already exists");

        client.FullName = clientDto.FullName;
        client.Phone = clientDto.Phone;
        client.Email = clientDto.Email;
        client.Discount = clientDto.Discount;
        client.Verified = clientDto.Verified;

        await _clientRepository.UpdateAsync(client);
    }

    public async Task PartialUpdateClientAsync(int id, ClientPartialUpdateRequestDto clientDto)
    {
        var client = await _clientRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Client not found");

        var patchDoc = new JsonPatchDocument<Client>();

        if (clientDto.FullName != null)
            patchDoc.Replace(c => c.FullName, clientDto.FullName);

        if (clientDto.Phone != null)
            patchDoc.Replace(c => c.Phone, clientDto.Phone);

        if (clientDto.Email != null)
        {
            var existing = _clientRepository.GetByEmailAsync(clientDto.Email);
            if (existing != null)
                throw new InvalidOperationException("Client with this email already exists");
            patchDoc.Replace(c => c.Email, clientDto.Email);
        }

        if (clientDto.Discount != 0)
            patchDoc.Replace(c => c.Discount, clientDto.Discount);

        if (clientDto.Verified != false)
            patchDoc.Replace(c => c.Verified, clientDto.Verified);

        if (patchDoc.Operations.Count == 0)
            throw new BadHttpRequestException("No fields to update.");

        patchDoc.ApplyTo(client);

        await _clientRepository.UpdateAsync(client);
    }

    public async Task DeleteClientAsync(int id)
    {
        _ = await _clientRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Client not found");

        await _clientRepository.DeleteAsync(id);
    }
}