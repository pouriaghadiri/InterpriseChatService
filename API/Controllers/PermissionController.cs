using Application.Features.AuthorizationUseCase.Commands;
using Application.Features.AuthorizationUseCase.Queries;
using Domain.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterpriseChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly ILogger<PermissionController> _logger;
    private readonly IMediator _mediator;

    public PermissionController(ILogger<PermissionController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new permission
    /// </summary>
    /// <param name="command">Permission creation data</param>
    /// <returns>Creation result</returns>
    [HttpPost("create")]
    public async Task<ActionResult<MessageDTO>> CreatePermission([FromBody] CreatePermissionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get permission by ID
    /// </summary>
    /// <param name="id">Permission ID</param>
    /// <returns>Permission details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPermissionById(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Permission ID is required");

        var result = await _mediator.Send(new GetPermissionByIdQuery(id));

        if (!result.IsSuccess)
            return NotFound(result.Message);

        return Ok(result);
    }

    /// <summary>
    /// Get all permissions
    /// </summary>
    /// <returns>List of all permissions</returns>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllPermissions()
    {
        var result = await _mediator.Send(new GetAllPermissionsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Update permission
    /// </summary>
    /// <param name="command">Permission update data</param>
    /// <returns>Update result</returns>
    [HttpPut("update")]
    public async Task<ActionResult<MessageDTO>> UpdatePermission([FromBody] UpdatePermissionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete permission
    /// </summary>
    /// <param name="id">Permission ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<MessageDTO>> DeletePermission(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Permission ID is required");

        var result = await _mediator.Send(new DeletePermissionCommand(id));
        return Ok(result);
    }

    /// <summary>
    /// Assign permission to role
    /// </summary>
    /// <param name="command">Permission assignment data</param>
    /// <returns>Assignment result</returns>
    [HttpPost("assign-to-role")]
    public async Task<ActionResult<MessageDTO>> AssignPermissionToRole([FromBody] AssignPermissionToRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Assign permission to user
    /// </summary>
    /// <param name="command">Permission assignment data</param>
    /// <returns>Assignment result</returns>
    [HttpPost("assign-to-user")]
    public async Task<ActionResult<MessageDTO>> AssignPermissionToUser([FromBody] AssignPermissionToUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get permissions for a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>List of role permissions</returns>
    [HttpGet("role/{roleId}/{departmentId}")]
    public async Task<IActionResult> GetRolePermissions(Guid roleId, Guid departmentId)
    {
        if (roleId == Guid.Empty)
            return BadRequest("Role ID is required");

        var result = await _mediator.Send(new GetRolePermissionsQuery(roleId, departmentId));
        return Ok(result);
    }

    /// <summary>
    /// Get permissions for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user permissions</returns>
    [HttpGet("user/{userId}/{departmentId}")]
    public async Task<IActionResult> GetUserPermissions(Guid userId, Guid departmentId)
    {
        if (userId == Guid.Empty)
            return BadRequest("User ID is required");

        var result = await _mediator.Send(new GetUserPermissionsQuery(userId, departmentId));
        return Ok(result);
    }
}
