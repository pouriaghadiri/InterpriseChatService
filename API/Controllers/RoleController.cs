using Application.Features.RoleUseCase.Commands;
using Application.Features.RoleUseCase.Queries;
using Domain.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterpriseChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class RoleController : ControllerBase
{
    private readonly ILogger<RoleController> _logger;
    private readonly IMediator _mediator;

    public RoleController(ILogger<RoleController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    /// <param name="command">Role creation command</param>
    /// <returns>Creation result</returns>
    [HttpPost("create")]
    public async Task<ActionResult<MessageDTO>> CreateRole([FromBody] AddRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get a role by ID
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>Role information</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleById(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Role ID is required");

        var result = await _mediator.Send(new GetRoleByIdQuery(id));

        if (!result.IsSuccess)
            return NotFound(result.Message);

        return Ok(result);
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    /// <returns>List of all roles</returns>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllRoles()
    {
        var result = await _mediator.Send(new GetAllRolesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Update an existing role
    /// </summary>
    /// <param name="command">Role update command</param>
    /// <returns>Update result</returns>
    [HttpPut("update")]
    public async Task<ActionResult<MessageDTO>> UpdateRole([FromBody] UpdateRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    /// <param name="id">Role ID to delete</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<MessageDTO>> DeleteRole(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Role ID is required");

        var result = await _mediator.Send(new DeleteRoleCommand(id));
        return Ok(result);
    }
}
