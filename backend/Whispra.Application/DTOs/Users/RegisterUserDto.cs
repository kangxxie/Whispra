namespace Whispra.Application.DTOs.Users;

public record RegisterUserDto(
    string Username,
    string Email,
    string Password
);