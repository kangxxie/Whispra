using Whispra.Application.DTOs.Users;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Domain.Entities.Users;

namespace Whispra.Application.UseCases.Users.Register;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponseDto> ExecuteAsync(
        RegisterUserDto dto,
        CancellationToken cancellationToken = default)
    {
        // Check if email already exists
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email already registered");
        }

        // Check if username already exists
        existingUser = await _userRepository.GetByUsernameAsync(dto.Username, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Username already taken");
        }

        // Create new user
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password)
        };

        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

        return new UserResponseDto(
            createdUser.Id,
            createdUser.Username,
            createdUser.Email,
            createdUser.DisplayName,
            createdUser.Bio,
            createdUser.ProfilePictureUrl,
            createdUser.CreatedAt
        );
    }
}