using API.DTOs.Auth;
using Application.Features.AuthenticationUseCase.Commands;
using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.UserUseCase;
using Application.Features.UserUseCase.Commands;
using Application.Features.UserUseCase.Queries;
using Domain.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace InterpriseChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IMediator _mediator;

    public AuthController(ILogger<UserController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
     
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ResultDTO<TokenResultDTO>>> Login([FromBody] LoginRequestDTO login)
    {
        var result = await _mediator.Send(new LoginCommand { Email = login.Email, Password = login.Password } );
        if (!result.IsSuccess)
        {
            Unauthorized(result.Message);
        }
        return Ok(result);
    }


    [HttpPost("register")]
    //[Authorize]
    public async Task<ActionResult<ResultDTO<Guid>>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("change-password")]
    [Authorize]
    public async Task<ActionResult<MessageDTO>> ChangePassword([FromBody] ChangePasswordUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }


}
