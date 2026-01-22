using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Communities;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class CommunityMemberRepository : ICommunityMemberRepository
{
    private readonly MongoDbContext _context;

    public CommunityMemberRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<CommunityMember?> GetMembershipAsync(
        string communityId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.CommunityMembers
            .Find(m => m.CommunityId == communityId && m.UserId == userId && !m.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CommunityMember> CreateAsync(
        CommunityMember member, CancellationToken cancellationToken = default)
    {
        await _context.CommunityMembers.InsertOneAsync(member, cancellationToken: cancellationToken);
        return member;
    }

    public async Task UpdateAsync(CommunityMember member, CancellationToken cancellationToken = default)
    {
        member.UpdatedAt = DateTime.UtcNow;
        await _context.CommunityMembers.ReplaceOneAsync(
            m => m.Id == member.Id,
            member,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var update = Builders<CommunityMember>.Update
            .Set(m => m.IsDeleted, true)
            .Set(m => m.DeletedAt, DateTime.UtcNow);

        await _context.CommunityMembers.UpdateOneAsync(
            m => m.Id == id,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task<List<CommunityMember>> GetCommunityMembersAsync(
        string communityId, CancellationToken cancellationToken = default)
    {
        return await _context.CommunityMembers
            .Find(m => m.CommunityId == communityId && !m.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetMemberCountAsync(
        string communityId, CancellationToken cancellationToken = default)
    {
        return (int)await _context.CommunityMembers
            .CountDocumentsAsync(
                m => m.CommunityId == communityId && !m.IsDeleted,
                cancellationToken: cancellationToken);
    }

    public async Task<bool> IsMemberAsync(
        string communityId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.CommunityMembers
            .Find(m => m.CommunityId == communityId && m.UserId == userId && !m.IsDeleted)
            .AnyAsync(cancellationToken);
    }
}