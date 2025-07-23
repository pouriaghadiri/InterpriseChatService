using Application.Features.UserUseCase;
using Application.Features.UserUseCase.Commands;
using Application.Features.UserUseCase.Queries;
using Domain.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InterpriseChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IMediator _mediator;

    public UserController(ILogger<UserController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
     
    [HttpPost("register")]
    public async Task<ActionResult<ResultDTO<Guid>>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
     
    [HttpPut("change-password")]
    public async Task<ActionResult<MessageDTO>> ChangePassword([FromBody] ChangePasswordUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
     
    [HttpPut("update-profile")]
    public async Task<ActionResult<MessageDTO>> UpdateProfile([FromBody] UpdateProfileUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get a user by email address
    /// </summary>
    /// <param name="email">User email address</param>
    /// <returns>User info</returns>
    [HttpGet("by-email")]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required");

        var result = await _mediator.Send(new GetUserByEmailQuery(email));

        if (!result.IsSuccess)
            return NotFound(result.Message); // or return BadRequest if you want to show validation error

        return Ok(result);
    }

}
