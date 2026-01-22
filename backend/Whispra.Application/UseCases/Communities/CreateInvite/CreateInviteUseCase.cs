using System.Security.Cryptography;
using Whispra.Application.DTOs.Communities;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Communities.CreateInvite;

public class CreateInviteUseCase
{
    private readonly ICommunityRepository _communityRepository;
    private readonly ICommunityMemberRepository _memberRepository;
    private readonly ICommunityInviteRepository _inviteRepository;

    public CreateInviteUseCase(
        ICommunityRepository communityRepository,
        ICommunityMemberRepository memberRepository,
        ICommunityInviteRepository inviteRepository)
    {
        _communityRepository = communityRepository;
        _memberRepository = memberRepository;
        _inviteRepository = inviteRepository;
    }

    public async Task<InviteResponseDto> ExecuteAsync(
        string communityId,
        CreateInviteDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var community = await _communityRepository.GetByIdAsync(communityId, cancellationToken);
        if (community == null)
        {
            throw new InvalidOperationException("Community not found");
        }

        // Check if user can create invites (moderator or owner)
        var membership = await _memberRepository.GetMembershipAsync(
            communityId, currentUserId, cancellationToken);

        if (membership == null)
        {
            throw new UnauthorizedAccessException("You are not a member of this community");
        }

        if (membership.Role != CommunityRole.Owner &&
            membership.Role != CommunityRole.Moderator)
        {
            throw new UnauthorizedAccessException("Only owners and moderators can create invites");
        }

        // Generate invite code
        var inviteCode = GenerateInviteCode();

        var invite = new CommunityInvite
        {
            CommunityId = communityId,
            InviteCode = inviteCode,
            CreatedByUserId = currentUserId,
            ExpiresAt = DateTime.UtcNow.AddDays(dto.ExpiresInDays ?? 7),
            MaxUses = dto.MaxUses
        };

        var createdInvite = await _inviteRepository.CreateAsync(invite, cancellationToken);

        return new InviteResponseDto(
            createdInvite.Id,
            createdInvite.InviteCode,
            createdInvite.CommunityId,
            createdInvite.ExpiresAt,
            createdInvite.MaxUses,
            createdInvite.UsesCount,
            createdInvite.IsActive
        );
    }

    private static string GenerateInviteCode()
    {
        var bytes = new byte[8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "")
            .ToUpper();
    }
}