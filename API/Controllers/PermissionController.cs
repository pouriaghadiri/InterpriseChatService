using Application.Features.AuthorizationUseCase.Commands;
using Application.Features.AuthorizationUseCase.Queries;
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
    [Authorize(Policy = "PERM_Permission_Create_Admin")]
    public async Task<ActionResult<MessageDTO>> CreatePermission([FromBody] CreatePermissionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get permission by ID (Admin only)
    /// </summary>
    /// <param name="id">Permission ID</param>
    /// <returns>Permission details</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "PERM_Permission_View_Admin")]
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
    [Authorize(Policy = "PERM_Permission_View_Admin")]
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
    [Authorize(Policy = "PERM_Permission_Update_Admin")]
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
    [Authorize(Policy = "PERM_Permission_Delete_Admin")]
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
    [Authorize(Policy = "PERM_Role_Permission_Assign_Admin")]
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
    [Authorize(Policy = "PERM_User_Permission_Assign_Admin")]
    public async Task<ActionResult<MessageDTO>> AssignPermissionToUser([FromBody] AssignPermissionToUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get current user's permissions (from JWT token)
    /// </summary>
    /// <param name="departmentId">Department ID (optional - uses active department if not provided)</param>
    /// <returns>List of current user's permissions</returns>
    [HttpGet("my")]
    [Authorize(Policy = "PERM_User_Permission_View_My")]
    public async Task<IActionResult> GetMyPermissions([FromQuery] Guid? departmentId = null)
    {
        var query = new GetMyPermissionsQuery
        {
            DepartmentId = departmentId ?? Guid.Empty
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get current user's role permissions (from JWT token)
    /// </summary>
    /// <param name="departmentId">Department ID (optional - uses active department if not provided)</param>
    /// <returns>List of current user's role permissions</returns>
    [HttpGet("my/roles")]
    [Authorize(Policy = "PERM_Role_Permission_View_My")]
    public async Task<IActionResult> GetMyRolePermissions([FromQuery] Guid? departmentId = null)
    {
        var query = new GetMyRolePermissionsQuery
        {
            DepartmentId = departmentId ?? Guid.Empty
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get permissions for a role (Admin only or if user has that role)
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="departmentId">Department ID</param>
    /// <returns>List of role permissions</returns>
    [HttpGet("role/{roleId}/{departmentId}")]
    [Authorize(Policy = "PERM_Role_Permission_View_Admin")]
    public async Task<IActionResult> GetRolePermissions(Guid roleId, Guid departmentId)
    {
        if (roleId == Guid.Empty)
            return BadRequest("Role ID is required");
        if (departmentId == Guid.Empty)
            return BadRequest("Department ID is required");

        var result = await _mediator.Send(new GetRolePermissionsQuery(roleId, departmentId));
        return Ok(result);
    }

    /// <summary>
    /// Get permissions for a user (Admin only - to view any user's permissions)
    /// Regular users should use GET /api/permission/my to view their own permissions
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="departmentId">Department ID</param>
    /// <returns>List of user permissions</returns>
    [HttpGet("user/{userId}/{departmentId}")]
    [Authorize(Policy = "PERM_User_Permission_View_Admin")]
    public async Task<IActionResult> GetUserPermissions(Guid userId, Guid departmentId)
    {
        if (userId == Guid.Empty)
            return BadRequest("User ID is required");
        if (departmentId == Guid.Empty)
            return BadRequest("Department ID is required");

        // Admin-only endpoint - no need to check if userId matches token
        // Regular users should use /my endpoint instead
        var result = await _mediator.Send(new GetUserPermissionsQuery(userId, departmentId));
        return Ok(result);
    }
}
