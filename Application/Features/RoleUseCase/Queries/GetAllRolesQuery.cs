using Application.Features.RoleUseCase.DTOs;
using Domain.Base;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.RoleUseCase.Queries
{
    public class GetAllRolesQuery : IRequest<ResultDTO<List<RoleDTO>>>
    {
    }
}
