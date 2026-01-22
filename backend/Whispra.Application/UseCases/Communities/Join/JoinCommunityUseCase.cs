using Whispra.Application.DTOs.Communities;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Communities.Join;

public class JoinCommunityUseCase
{
    private readonly ICommunityRepository _communityRepository;
    private readonly ICommunityMemberRepository _memberRepository;
    private readonly ICommunityInviteRepository _inviteRepository;

    public JoinCommunityUseCase(
        ICommunityRepository communityRepository,
        ICommunityMemberRepository memberRepository,
        ICommunityInviteRepository inviteRepository)
    {
        _communityRepository = communityRepository;
        _memberRepository = memberRepository;
        _inviteRepository = inviteRepository;
    }

    public async Task<CommunityResponseDto> ExecuteAsync(
        string communityId,
        string currentUserId,
        JoinCommunityDto dto,
        CancellationToken cancellationToken = default)
    {
        var community = await _communityRepository.GetByIdAsync(communityId, cancellationToken);
        if (community == null)
        {
            throw new InvalidOperationException("Community not found");
        }

        // Check if already a member
        var existingMembership = await _memberRepository.GetMembershipAsync(
            communityId, currentUserId, cancellationToken);
        if (existingMembership != null)
        {
            throw new InvalidOperationException("Already a member of this community");
        }

        // If private, validate invite code
        if (community.Privacy == CommunityPrivacy.Private)
        {
            if (string.IsNullOrEmpty(dto.InviteCode))
            {
                throw new InvalidOperationException("Invite code required for private community");
            }

            var invite = await _inviteRepository.GetByCodeAsync(dto.InviteCode, cancellationToken);
            if (invite == null || !invite.IsActive || invite.CommunityId != communityId)
            {
                throw new InvalidOperationException("Invalid invite code");
            }

            if (invite.ExpiresAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Invite code expired");
            }

            if (invite.MaxUses.HasValue && invite.UsesCount >= invite.MaxUses.Value)
            {
                throw new InvalidOperationException("Invite code has reached max uses");
            }

            // Increment invite uses
            invite.UsesCount++;
            await _inviteRepository.UpdateAsync(invite, cancellationToken);
        }

        // Add member
        var membership = new CommunityMember
        {
            CommunityId = communityId,
            UserId = currentUserId,
            Role = CommunityRole.Member
        };

        await _memberRepository.CreateAsync(membership, cancellationToken);

        // Update member count
        community.MemberCount++;
        await _communityRepository.UpdateAsync(community, cancellationToken);

        return new CommunityResponseDto(
            community.Id,
            community.Name,
            community.Description,
            community.CoverImageUrl,
            community.Privacy,
            community.OwnerId,
            community.MemberCount,
            community.Tags,
            community.CreatedAt,
            CommunityRole.Member
        );
    }
}