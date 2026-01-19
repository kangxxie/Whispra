using Whispra.Application.DTOs.Auth;
using Whispra.Application.DTOs.Users;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Domain.Entities.Auth;

namespace Whispra.Application.UseCases.Auth.RefreshTokens;

public class RefreshTokenUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenUseCase(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> ExecuteAsync(
        RefreshTokenDto dto,
        CancellationToken cancellationToken = default)
    {
        // Find refresh token
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(
            dto.RefreshToken, cancellationToken);

        if (storedToken == null || storedToken.IsRevoked)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        // Check if expired
        if (storedToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token expired");
        }

        // Get user
        var user = await _userRepository.GetByIdAsync(storedToken.UserId, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        // Generate new tokens
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // Revoke old refresh token and create new one (rotation)
        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedByToken = newRefreshToken;
        await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);

        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = _jwtTokenService.GetRefreshTokenExpiration()
        };
        await _refreshTokenRepository.CreateAsync(newRefreshTokenEntity, cancellationToken);

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
            newAccessToken,
            newRefreshToken,
            _jwtTokenService.GetAccessTokenExpiration(),
            _jwtTokenService.GetRefreshTokenExpiration(),
            userResponse
        );
    }
}