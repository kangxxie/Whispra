using Whispra.Application.DTOs.Communities;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Communities.UpdateRole;

public class UpdateMemberRoleUseCase
{
    private readonly ICommunityMemberRepository _memberRepository;

    public UpdateMemberRoleUseCase(ICommunityMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task ExecuteAsync(
        string communityId,
        UpdateMemberRoleDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        // Check if current user is owner or moderator
        var currentUserMembership = await _memberRepository.GetMembershipAsync(
            communityId, currentUserId, cancellationToken);

        if (currentUserMembership == null)
        {
            throw new UnauthorizedAccessException("You are not a member of this community");
        }

        if (currentUserMembership.Role != CommunityRole.Owner &&
            currentUserMembership.Role != CommunityRole.Moderator)
        {
            throw new UnauthorizedAccessException("Only owners and moderators can change roles");
        }

        // Cannot change owner role (would need ownership transfer use case)
        if (dto.NewRole == CommunityRole.Owner)
        {
            throw new InvalidOperationException("Cannot assign owner role. Use transfer ownership instead.");
        }

        // Get target member
        var targetMembership = await _memberRepository.GetMembershipAsync(
            communityId, dto.UserId, cancellationToken);

        if (targetMembership == null)
        {
            throw new InvalidOperationException("User is not a member of this community");
        }

        // Moderators cannot change other moderators' or owner's roles
        if (currentUserMembership.Role == CommunityRole.Moderator &&
            targetMembership.Role != CommunityRole.Member)
        {
            throw new UnauthorizedAccessException("Moderators can only change regular members' roles");
        }

        // Update role
        targetMembership.Role = dto.NewRole;
        await _memberRepository.UpdateAsync(targetMembership, cancellationToken);
    }
}