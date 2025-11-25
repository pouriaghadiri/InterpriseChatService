using Application.Features.DepartmentUseCase.DTOs;
using Domain.Base;
using MediatR;

namespace Application.Features.DepartmentUseCase.Queries
{
    /// <summary>
    /// Query to get current user's departments (from JWT token)
    /// </summary>
    public class GetMyDepartmentsQuery : IRequest<ResultDTO<List<DepartmentDTO>>>
    {
    }
}

