using Application.Features.RoleUseCase.DTOs;
using Domain.Base;
using MediatR;

namespace Application.Features.RoleUseCase.Queries
{
    /// <summary>
    /// Query to get current user's roles (from JWT token)
    /// </summary>
    public class GetMyRolesQuery : IRequest<ResultDTO<List<RoleDTO>>>
    {
    }
}

