namespace Whispra.Application.DTOs.Communities;

public record JoinCommunityDto(
    string? InviteCode = null // Required for private communities
);