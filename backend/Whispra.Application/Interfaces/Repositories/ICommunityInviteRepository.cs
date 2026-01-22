using Whispra.Domain.Entities.Communities;

namespace Whispra.Application.Interfaces.Repositories;

public interface ICommunityInviteRepository
{
    Task<CommunityInvite?> GetByCodeAsync(string inviteCode, CancellationToken cancellationToken = default);
    Task<CommunityInvite> CreateAsync(CommunityInvite invite, CancellationToken cancellationToken = default);
    Task UpdateAsync(CommunityInvite invite, CancellationToken cancellationToken = default);
    Task<List<CommunityInvite>> GetCommunityInvitesAsync(string communityId, CancellationToken cancellationToken = default);
}