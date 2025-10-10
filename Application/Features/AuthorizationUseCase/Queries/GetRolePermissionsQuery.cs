using Application.Features.AuthorizationUseCase.DTOs;
using Domain.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Queries
{
    public record GetRolePermissionsQuery(Guid roleId):IRequest<ResultDTO<List<PermissionDTO>>>
    {
    }
}
