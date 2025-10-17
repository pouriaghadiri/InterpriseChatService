using Application.Features.DepartmentUseCase.Commands;
using Application.Features.DepartmentUseCase.Queries;
using Domain.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterpriseChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentController : ControllerBase
{
    private readonly ILogger<DepartmentController> _logger;
    private readonly IMediator _mediator;

    public DepartmentController(ILogger<DepartmentController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new department
    /// </summary>
    /// <param name="command">Department creation command</param>
    /// <returns>Creation result</returns>
    [HttpPost("create")]
    public async Task<MessageDTO> CreateDepartment([FromBody] AddDepartmentCommand command)
    {
        var result = await _mediator.Send(command);
        return result;
    }

    /// <summary>
    /// Get a department by ID
    /// </summary>
    /// <param name="id">Department ID</param>
    /// <returns>Department information</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDepartmentById(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Department ID is required");

        var result = await _mediator.Send(new GetDepartmentByIdQuery(id));

        if (!result.IsSuccess)
            return NotFound(result.Message);

        return Ok(result);
    }

    /// <summary>
    /// Get all departments
    /// </summary>
    /// <returns>List of all departments</returns>
    [Authorize(Policy = "PERM_Department_All_View")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllDepartments()
    {
        var result = await _mediator.Send(new GetAllDepartmentsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Update an existing department
    /// </summary>
    /// <param name="command">Department update command</param>
    /// <returns>Update result</returns>
    [HttpPut("update")]
    public async Task<ActionResult<MessageDTO>> UpdateDepartment([FromBody] UpdateDepartmentCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a department
    /// </summary>
    /// <param name="id">Department ID to delete</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<MessageDTO>> DeleteDepartment(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Department ID is required");

        var result = await _mediator.Send(new DeleteDepartmentCommand(id));
        return Ok(result);
    }
}
