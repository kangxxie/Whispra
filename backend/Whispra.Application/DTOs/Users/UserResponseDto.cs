namespace Whispra.Application.DTOs.Users;

public record UserResponseDto(
    string Id,
    string Username,
    string Email,
    string? DisplayName,
    string? Bio,
    string? ProfilePictureUrl,
    DateTime CreatedAt
);