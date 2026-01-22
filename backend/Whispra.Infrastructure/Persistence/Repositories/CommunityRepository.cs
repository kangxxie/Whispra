using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Communities;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class CommunityRepository : ICommunityRepository
{
    private readonly MongoDbContext _context;

    public CommunityRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Community?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Communities
            .Find(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Community> CreateAsync(Community community, CancellationToken cancellationToken = default)
    {
        await _context.Communities.InsertOneAsync(community, cancellationToken: cancellationToken);
        return community;
    }

    public async Task UpdateAsync(Community community, CancellationToken cancellationToken = default)
    {
        community.UpdatedAt = DateTime.UtcNow;
        await _context.Communities.ReplaceOneAsync(
            c => c.Id == community.Id,
            community,
            cancellationToken: cancellationToken);
    }

    public async Task<List<Community>> GetPublicCommunitiesAsync(
        int skip, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Communities
            .Find(c => !c.IsDeleted && c.Privacy == Domain.Enums.CommunityPrivacy.Public)
            .SortByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Community>> GetUserCommunitiesAsync(
        string userId, int skip, int limit, CancellationToken cancellationToken = default)
    {
        // This requires joining with CommunityMember collection
        var membershipIds = await _context.CommunityMembers
            .Find(m => m.UserId == userId) // get memberships for the user 
            .Project(m => m.CommunityId) // project to community IDs
            .ToListAsync(cancellationToken); // execute the query

        return await _context.Communities
            .Find(c => membershipIds.Contains(c.Id) && !c.IsDeleted)
            .SortByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Communities
            .Find(c => c.Id == id && !c.IsDeleted)
            .AnyAsync(cancellationToken);
    }
}