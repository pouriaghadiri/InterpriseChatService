using Application.Features.AuthorizationUseCase.DTOs;
using Domain.Base;
using Domain.Repositories;
using MediatR;

namespace Application.Features.AuthorizationUseCase.Queries
{
    public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, ResultDTO<List<PermissionDTO>>>
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetAllPermissionsQueryHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<ResultDTO<List<PermissionDTO>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _permissionRepository.GetAllAsync();
            
            var permissionDtos = permissions.Select(p => new PermissionDTO
            {
                Id = p.Id,
                Name = p.Name.Value,
                Description = p.Description
            }).ToList();

            return ResultDTO<List<PermissionDTO>>.Success("Retrieved", permissionDtos, "All permissions retrieved successfully");
        }
    }
}
