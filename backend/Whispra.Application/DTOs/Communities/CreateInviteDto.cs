namespace Whispra.Application.DTOs.Communities;

public record CreateInviteDto(
    int? MaxUses = null,
    int? ExpiresInDays = 7
);