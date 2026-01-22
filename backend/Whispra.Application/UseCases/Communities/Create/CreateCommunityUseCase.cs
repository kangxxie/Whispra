using Whispra.Application.DTOs.Communities;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Communities.Create;

public class CreateCommunityUseCase
{
    private readonly ICommunityRepository _communityRepository;
    private readonly ICommunityMemberRepository _memberRepository;

    public CreateCommunityUseCase(
        ICommunityRepository communityRepository,
        ICommunityMemberRepository memberRepository)
    {
        _communityRepository = communityRepository;
        _memberRepository = memberRepository;
    }

    public async Task<CommunityResponseDto> ExecuteAsync(
        CreateCommunityDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        // Create community
        var community = new Community
        {
            Name = dto.Name,
            Description = dto.Description,
            Privacy = dto.Privacy,
            OwnerId = currentUserId,
            MemberCount = 1,
            Tags = dto.Tags ?? new List<string>()
        };

        var createdCommunity = await _communityRepository.CreateAsync(community, cancellationToken);

        // Add creator as owner/member
        var membership = new CommunityMember
        {
            CommunityId = createdCommunity.Id,
            UserId = currentUserId,
            Role = CommunityRole.Owner
        };

        await _memberRepository.CreateAsync(membership, cancellationToken);

        return new CommunityResponseDto(
            createdCommunity.Id,
            createdCommunity.Name,
            createdCommunity.Description,
            createdCommunity.CoverImageUrl,
            createdCommunity.Privacy,
            createdCommunity.OwnerId,
            createdCommunity.MemberCount,
            createdCommunity.Tags,
            createdCommunity.CreatedAt,
            CommunityRole.Owner
        );
    }
}