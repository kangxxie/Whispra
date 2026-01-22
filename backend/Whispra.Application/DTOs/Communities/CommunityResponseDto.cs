using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Communities;

public record CommunityResponseDto(
    string Id,
    string Name,
    string? Description,
    string? CoverImageUrl,
    CommunityPrivacy Privacy,
    string OwnerId,
    int MemberCount,
    List<string> Tags,
    DateTime CreatedAt,
    CommunityRole? CurrentUserRole = null // Will be populated if user is a member
);