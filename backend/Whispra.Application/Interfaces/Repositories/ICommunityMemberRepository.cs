using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Enums;

namespace Whispra.Application.Interfaces.Repositories;

public interface ICommunityMemberRepository
{
    Task<CommunityMember?> GetMembershipAsync(string communityId, string userId, CancellationToken cancellationToken = default);
    Task<CommunityMember> CreateAsync(CommunityMember member, CancellationToken cancellationToken = default);
    Task UpdateAsync(CommunityMember member, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<List<CommunityMember>> GetCommunityMembersAsync(string communityId, CancellationToken cancellationToken = default);
    Task<int> GetMemberCountAsync(string communityId, CancellationToken cancellationToken = default);
    Task<bool> IsMemberAsync(string communityId, string userId, CancellationToken cancellationToken = default);
}