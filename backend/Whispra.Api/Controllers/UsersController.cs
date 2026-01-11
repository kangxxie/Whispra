using Microsoft.AspNetCore.Mvc;
using Whispra.Application.DTOs.Users;
using Whispra.Application.UseCases.Users.Register;

namespace Whispra.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase;

    public UsersController(RegisterUserUseCase registerUserUseCase)
    {
        _registerUserUseCase = registerUserUseCase;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register(
        [FromBody] RegisterUserDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _registerUserUseCase.ExecuteAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}