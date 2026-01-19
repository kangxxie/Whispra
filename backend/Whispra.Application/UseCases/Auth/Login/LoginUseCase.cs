using Whispra.Application.DTOs.Auth;
using Whispra.Application.DTOs.Users;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Domain.Entities.Auth;

namespace Whispra.Application.UseCases.Auth.Login;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUseCase(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> ExecuteAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Store refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = _jwtTokenService.GetRefreshTokenExpiration()
        };
        await _refreshTokenRepository.CreateAsync(refreshTokenEntity, cancellationToken);

        var userResponse = new UserResponseDto(
            user.Id,
            user.Username,
            user.Email,
            user.DisplayName,
            user.Bio,
            user.ProfilePictureUrl,
            user.CreatedAt
        );

        return new AuthResponseDto(
            accessToken,
            refreshToken,
            _jwtTokenService.GetAccessTokenExpiration(),
            _jwtTokenService.GetRefreshTokenExpiration(),
            userResponse
        );
    }
}