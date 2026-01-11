using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MongoDbContext _context;

    public UserRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Find(u => u.Id == id && !u.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Find(u => u.Email == email && !u.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Find(u => u.Username == username && !u.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);
        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        user.UpdatedAt = DateTime.UtcNow;
        await _context.Users.ReplaceOneAsync(
            u => u.Id == user.Id,
            user,
            cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Find(u => u.Id == id && !u.IsDeleted)
            .AnyAsync(cancellationToken);
    }
}