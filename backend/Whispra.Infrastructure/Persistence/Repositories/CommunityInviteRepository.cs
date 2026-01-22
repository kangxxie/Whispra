using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Communities;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class CommunityInviteRepository : ICommunityInviteRepository
{
    private readonly MongoDbContext _context;

    public CommunityInviteRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<CommunityInvite?> GetByCodeAsync(
        string inviteCode, CancellationToken cancellationToken = default)
    {
        return await _context.CommunityInvites
            .Find(i => i.InviteCode == inviteCode && !i.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CommunityInvite> CreateAsync(
        CommunityInvite invite, CancellationToken cancellationToken = default)
    {
        await _context.CommunityInvites.InsertOneAsync(invite, cancellationToken: cancellationToken);
        return invite;
    }

    public async Task UpdateAsync(CommunityInvite invite, CancellationToken cancellationToken = default)
    {
        invite.UpdatedAt = DateTime.UtcNow;
        await _context.CommunityInvites.ReplaceOneAsync(
            i => i.Id == invite.Id,
            invite,
            cancellationToken: cancellationToken);
    }

    public async Task<List<CommunityInvite>> GetCommunityInvitesAsync(
        string communityId, CancellationToken cancellationToken = default)
    {
        return await _context.CommunityInvites
            .Find(i => i.CommunityId == communityId && !i.IsDeleted && i.IsActive)
            .ToListAsync(cancellationToken);
    }
}