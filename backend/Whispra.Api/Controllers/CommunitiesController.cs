using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whispra.Application.DTOs.Communities;
using Whispra.Application.UseCases.Communities.Create;
using Whispra.Application.UseCases.Communities.CreateInvite;
using Whispra.Application.UseCases.Communities.Join;
using Whispra.Application.UseCases.Communities.Leave;
using Whispra.Application.UseCases.Communities.UpdateRole;

namespace Whispra.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CommunitiesController : ControllerBase
{
    private readonly CreateCommunityUseCase _createCommunityUseCase;
    private readonly JoinCommunityUseCase _joinCommunityUseCase;
    private readonly LeaveCommunityUseCase _leaveCommunityUseCase;
    private readonly UpdateMemberRoleUseCase _updateMemberRoleUseCase;
    private readonly CreateInviteUseCase _createInviteUseCase;

    public CommunitiesController(
        CreateCommunityUseCase createCommunityUseCase,
        JoinCommunityUseCase joinCommunityUseCase,
        LeaveCommunityUseCase leaveCommunityUseCase,
        UpdateMemberRoleUseCase updateMemberRoleUseCase,
        CreateInviteUseCase createInviteUseCase)
    {
        _createCommunityUseCase = createCommunityUseCase;
        _joinCommunityUseCase = joinCommunityUseCase;
        _leaveCommunityUseCase = leaveCommunityUseCase;
        _updateMemberRoleUseCase = updateMemberRoleUseCase;
        _createInviteUseCase = createInviteUseCase;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User not authenticated");
    }

    [HttpPost]
    public async Task<ActionResult<CommunityResponseDto>> CreateCommunity(
        [FromBody] CreateCommunityDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createCommunityUseCase.ExecuteAsync(
                dto, GetCurrentUserId(), cancellationToken);
            return CreatedAtAction(nameof(CreateCommunity), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{communityId}/join")]
    public async Task<ActionResult<CommunityResponseDto>> JoinCommunity(
        string communityId,
        [FromBody] JoinCommunityDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _joinCommunityUseCase.ExecuteAsync(
                communityId, GetCurrentUserId(), dto, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{communityId}/leave")]
    public async Task<ActionResult> LeaveCommunity(
        string communityId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _leaveCommunityUseCase.ExecuteAsync(
                communityId, GetCurrentUserId(), cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{communityId}/members/role")]
    public async Task<ActionResult> UpdateMemberRole(
        string communityId,
        [FromBody] UpdateMemberRoleDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _updateMemberRoleUseCase.ExecuteAsync(
                communityId, dto, GetCurrentUserId(), cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{communityId}/invites")]
    public async Task<ActionResult<InviteResponseDto>> CreateInvite(
        string communityId,
        [FromBody] CreateInviteDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createInviteUseCase.ExecuteAsync(
                communityId, dto, GetCurrentUserId(), cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}