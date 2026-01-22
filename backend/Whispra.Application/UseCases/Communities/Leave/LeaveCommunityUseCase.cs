using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Communities.Leave;

public class LeaveCommunityUseCase
{
    private readonly ICommunityRepository _communityRepository;
    private readonly ICommunityMemberRepository _memberRepository;

    public LeaveCommunityUseCase(
        ICommunityRepository communityRepository,
        ICommunityMemberRepository memberRepository)
    {
        _communityRepository = communityRepository;
        _memberRepository = memberRepository;
    }

    public async Task ExecuteAsync(
        string communityId,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var community = await _communityRepository.GetByIdAsync(communityId, cancellationToken);
        if (community == null)
        {
            throw new InvalidOperationException("Community not found");
        }

        var membership = await _memberRepository.GetMembershipAsync(
            communityId, currentUserId, cancellationToken);
        if (membership == null)
        {
            throw new InvalidOperationException("Not a member of this community");
        }

        // Owner cannot leave (must transfer ownership or delete community first)
        if (membership.Role == CommunityRole.Owner)
        {
            throw new InvalidOperationException("Owner cannot leave community. Transfer ownership or delete the community first.");
        }

        // Remove membership
        await _memberRepository.DeleteAsync(membership.Id, cancellationToken);

        // Update member count
        community.MemberCount--;
        await _communityRepository.UpdateAsync(community, cancellationToken);
    }
}