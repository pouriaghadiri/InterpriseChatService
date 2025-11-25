using Application.Features.AuthorizationUseCase.DTOs;
using Domain.Base;
using MediatR;

namespace Application.Features.AuthorizationUseCase.Queries
{
    /// <summary>
    /// Query to get current user's permissions (from JWT token)
    /// </summary>
    public class GetMyPermissionsQuery : IRequest<ResultDTO<List<PermissionDTO>>>
    {
        public Guid DepartmentId { get; set; }
    }
}

