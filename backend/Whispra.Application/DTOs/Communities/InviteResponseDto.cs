namespace Whispra.Application.DTOs.Communities;

public record InviteResponseDto(
    string Id,
    string InviteCode,
    string CommunityId,
    DateTime ExpiresAt,
    int? MaxUses,
    int UsesCount,
    bool IsActive
);