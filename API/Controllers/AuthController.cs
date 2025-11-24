using API.DTOs.Auth;
using Application.Features.AuthenticationUseCase.Commands;
using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Queries;
using Application.Features.UserUseCase.Commands;
using Domain.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InterpriseChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IMediator _mediator;

    public AuthController(ILogger<AuthController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="login">Login credentials</param>
    /// <returns>JWT token and expiration</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<ActionResult<ResultDTO<TokenResultDTO>>> Login([FromBody] LoginRequestDTO login)
    {
        var result = await _mediator.Send(new LoginCommand { Email = login.Email, Password = login.Password });
        if (!result.IsSuccess)
        {
            return Unauthorized(result.Message);
        }
        return Ok(result);
    }

    /// <summary>
    /// User registration
    /// </summary>
    /// <param name="command">User registration data</param>
    /// <returns>Registration result</returns>
    [HttpPost("register")]
    //[AllowAnonymous]
    [EnableRateLimiting("AuthPolicy")]
    [Authorize(Policy = "PERM_Register_User")]
    public async Task<ActionResult<ResultDTO<Guid>>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="command">Password change data</param>
    /// <returns>Change result</returns>
    [HttpPut("change-password")]
    [Authorize(Policy = "PERM_Change_Password")]
    public async Task<ActionResult<MessageDTO>> ChangePassword([FromBody] ChangePasswordUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Logout user (invalidate token)
    /// </summary>
    /// <returns>Logout result</returns>
    [HttpPost("logout")]
    [Authorize (Policy = "PERM_Logout")]
    public async Task<ActionResult<MessageDTO>> Logout()
    {
        var result = await _mediator.Send(new LogoutCommand());
        return Ok(result);
    }

    /// <summary>
    /// Refresh JWT token
    /// </summary>
    /// <param name="command">Refresh token data</param>
    /// <returns>New JWT token</returns>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ResultDTO<TokenResultDTO>>> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return Unauthorized(result.Message);
        }
        return Ok(result);
    }

    /// <summary>
    /// Forgot password - send reset email
    /// </summary>
    /// <param name="command">Email for password reset</param>
    /// <returns>Reset result</returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageDTO>> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Reset password with token
    /// </summary>
    /// <param name="command">Password reset data</param>
    /// <returns>Reset result</returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageDTO>> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Verify email address
    /// </summary>
    /// <param name="command">Email verification data</param>
    /// <returns>Verification result</returns>
    [HttpPost("verify-email")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageDTO>> VerifyEmail([FromBody] VerifyEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Resend email verification
    /// </summary>
    /// <param name="command">Email for verification</param>
    /// <returns>Resend result</returns>
    [HttpPost("resend-verification")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageDTO>> ResendVerification([FromBody] ResendVerificationCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>User profile</returns>
    [HttpGet("profile")]
    [Authorize (Policy = "PERM_Review_My_Profile")]
    public async Task<ActionResult<ResultDTO<UserProfileDTO>>> GetProfile()
    {
        var result = await _mediator.Send(new GetUserProfileQuery());
        return Ok(result);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="command">Profile update data</param>
    /// <returns>Update result</returns>
    [HttpPut("profile")]
    [Authorize (Policy = "PERM_Update_My_Profile")]
    public async Task<ActionResult<MessageDTO>> UpdateProfile([FromBody] UpdateProfileUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
