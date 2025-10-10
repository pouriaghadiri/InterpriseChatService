using Application.Features.AuthorizationUseCase.DTOs;
using Domain.Base;
using Domain.Repositories;
using MediatR;

namespace Application.Features.AuthorizationUseCase.Queries
{
    public class GetPermissionByIdQueryHandler : IRequestHandler<GetPermissionByIdQuery, ResultDTO<PermissionDTO>>
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetPermissionByIdQueryHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<ResultDTO<PermissionDTO>> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
        {
            var permission = await _permissionRepository.GetbyIdAsync(request.Id);
            if (permission == null)
            {
                return ResultDTO<PermissionDTO>.Failure("Not Found", null, "Permission not found");
            }

            var permissionDto = new PermissionDTO
            {
                Id = permission.Id,
                Name = permission.Name.Value,
                Description = permission.Description
            };

            return ResultDTO<PermissionDTO>.Success("Found", permissionDto, "Permission retrieved successfully");
        }
    }
}
