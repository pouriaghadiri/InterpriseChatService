using Application.Features.RoleUseCase.DTOs;
using Domain.Base;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.RoleUseCase.Queries
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, ResultDTO<List<RoleDTO>>>
    {
        private readonly IRoleRepository _roleRepository;

        public GetAllRolesQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<ResultDTO<List<RoleDTO>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetAllAsync();
            
            if (roles == null || !roles.Any())
            {
                return ResultDTO<List<RoleDTO>>.Success("Retrieved", new List<RoleDTO>(), "No roles found.");
            }

            var roleDTOs = roles.Select(role => new RoleDTO
            {
                Id = role.Id,
                Name = role.Name.Value,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            }).ToList();

            return ResultDTO<List<RoleDTO>>.Success("Retrieved", roleDTOs, "Roles retrieved successfully.");
        }
    }
}
