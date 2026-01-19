namespace Whispra.Application.DTOs.Auth;

public record LoginDto(
    string Email,
    string Password
);