using Application.Features.AuthorizationUseCase.DTOs;
using Domain.Base;
using MediatR;

namespace Application.Features.AuthorizationUseCase.Queries
{
    public class GetAllPermissionsQuery : IRequest<ResultDTO<List<PermissionDTO>>>
    {
        // This query will retrieve all permissions
    }
}
