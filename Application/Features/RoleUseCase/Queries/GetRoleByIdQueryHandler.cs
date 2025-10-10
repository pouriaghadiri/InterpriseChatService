using Application.Features.RoleUseCase.DTOs;
using Domain.Base;
using Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.RoleUseCase.Queries
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, ResultDTO<RoleDTO>>
    {
        private readonly IRoleRepository _roleRepository;

        public GetRoleByIdQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<ResultDTO<RoleDTO>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetbyIdAsync(request.Id);
            
            if (role == null)
            {
                return ResultDTO<RoleDTO>.Failure("Not Found", null, "Role not found.");
            }

            var roleDTO = new RoleDTO
            {
                Id = role.Id,
                Name = role.Name.Value,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            };

            return ResultDTO<RoleDTO>.Success("Retrieved", roleDTO, "Role retrieved successfully.");
        }
    }
}
