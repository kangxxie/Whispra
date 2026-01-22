using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Communities;

public record CreateCommunityDto(
    string Name,
    string? Description,
    CommunityPrivacy Privacy,
    List<string>? Tags
);