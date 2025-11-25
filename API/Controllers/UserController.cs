using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Queries;
using Application.Features.UserUseCase;
using Application.Features.UserUseCase.Commands;
using Application.Features.UserUseCase.Queries;
using Domain.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InterpriseChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("ApiPolicy")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IMediator _mediator;

    public UserController(ILogger<UserController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get current user's profile (from JWT token)
    /// </summary>
    /// <returns>Current user's profile</returns>
    /// <summary>
    /// Get current user's profile (from JWT token)
    /// </summary>
    /// <returns>Current user's profile</returns>
    [HttpGet("my")]
    [Authorize(Policy = "PERM_Profile_View_My")]
    public async Task<ActionResult<ResultDTO<UserProfileDTO>>> GetMyProfile()
    {
        var result = await _mediator.Send(new GetUserProfileQuery());
        return Ok(result);
    }

    /// <summary>
    /// Update current user's profile (from JWT token)
    /// </summary>
    /// <param name="command">Profile update data</param>
    /// <returns>Update result</returns>
    [HttpPut("my")]
    [Authorize(Policy = "PERM_Profile_Update_My")]
    public async Task<ActionResult<MessageDTO>> UpdateMyProfile([FromBody] UpdateMyProfileCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get a user by ID (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User information</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "PERM_User_View_Admin")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("User ID is required");

        var result = await _mediator.Send(new GetUserByIdQuery(id));

        if (!result.IsSuccess)
            return NotFound(result.Message);

        return Ok(result);
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet("all")]
    [Authorize(Policy = "PERM_User_View_Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());
        return Ok(result);
    }

    /// <summary>
    /// Get a user by email address (Admin only)
    /// </summary>
    /// <param name="email">User email address</param>
    /// <returns>User info</returns>
    [HttpGet("by-email")]
    [Authorize(Policy = "PERM_User_View_Admin")]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required");

        var result = await _mediator.Send(new GetUserByEmailQuery(email));

        if (!result.IsSuccess)
            return NotFound(result.Message);

        return Ok(result);
    }

    /// <summary>
    /// Update any user's profile (Admin only)
    /// </summary>
    /// <param name="command">Profile update data</param>
    /// <returns>Update result</returns>
    [HttpPut("update")]
    [Authorize(Policy = "PERM_User_Update_Admin")]
    public async Task<ActionResult<MessageDTO>> UpdateUser([FromBody] UpdateProfileUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a user (Admin only)
    /// </summary>
    /// <param name="id">User ID to delete</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "PERM_User_Delete_Admin")]
    public async Task<ActionResult<MessageDTO>> DeleteUser(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("User ID is required");

        var result = await _mediator.Send(new DeleteUserCommand { Id = id });
        return Ok(result);
    }

    /// <summary>
    /// Assign role to user (Admin only)
    /// </summary>
    /// <param name="command">Role assignment data</param>
    /// <returns>Assignment result</returns>
    [HttpPost("assign-role")]
    [Authorize(Policy = "PERM_User_Role_Assign_Admin")]
    public async Task<ActionResult<MessageDTO>> AssignRoleToUser([FromBody] AssignRoleToUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
