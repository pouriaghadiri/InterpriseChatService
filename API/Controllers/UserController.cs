using Application.Features.UserUseCase;
using Application.Features.UserUseCase.Commands;
using Domain.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InterpriseChatService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{ 
    private readonly ILogger<UserController> _logger;
    private readonly IMediator _mediator;

    public UserController(ILogger<UserController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<ActionResult<ResultDTO<Guid>>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    [HttpPost]
    [Route("ChnagePassword")]
    public async Task<ActionResult<MessageDTO>> ChnagePassword([FromBody] ChangePasswordUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    [HttpPost]
    [Route("UpdateProfile")]
    public async Task<ActionResult<MessageDTO>> UpdateProfile([FromBody] UpdateProfileUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

}
