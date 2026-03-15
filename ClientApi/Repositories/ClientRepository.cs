using ClientApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientApi.Repositories;

public class ClientRepository(ApplicationDbContext context) : IClientRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        return await _context.Clients.ToListAsync();
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        return await _context.Clients.FindAsync(id);
    }

    public async Task<Client?> GetByEmailAsync(string email)
    {
        return await _context.Clients.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Client> AddAsync(Client client)
    {
        var clientDb = await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();

        return clientDb.Entity;
    }

    public async Task UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);

        if (client != null)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }
    }
}