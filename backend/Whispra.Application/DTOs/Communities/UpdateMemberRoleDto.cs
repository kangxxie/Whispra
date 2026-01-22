using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Communities;

public record UpdateMemberRoleDto(
    string UserId,
    CommunityRole NewRole
);