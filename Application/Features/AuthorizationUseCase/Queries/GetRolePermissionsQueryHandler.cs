using Application.Features.AuthorizationUseCase.DTOs;
using Domain.Base;
using Domain.Base.Interface;
using MediatR;

namespace Application.Features.AuthorizationUseCase.Queries
{
    public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, ResultDTO<List<PermissionDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetRolePermissionsQueryHandler(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResultDTO<List<PermissionDTO>>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
        {
            var roleExist = await _unitOfWork.Users.ExistsAsync(x => x.Id == request.roleId, cancellationToken);
            if (!roleExist)
            {
                return ResultDTO<List<PermissionDTO>>.Failure("Not Exist Error", new List<string> { "The selected role doesn't have exist!" }, "Please select a valid role.");
            }
            var rolePermissions = await _unitOfWork.RolePermissions.GetRolePermissionsAsync(request.roleId, request.departmentId, cancellationToken);
            if (rolePermissions == null || !rolePermissions.Any())
            {
                return ResultDTO<List<PermissionDTO>>.Failure("No Permissions", new List<string> { "The role has no assigned permissions." }, "No permissions found for the role.");
            }
            var permissionDTOs = rolePermissions.Select(s => s.Permission)
                                                .Select(p => new PermissionDTO
            {
                Id = p.Id,
                Name = p.Name.Value,
                Description = p.Description
            }).ToList();
            return ResultDTO<List<PermissionDTO>>.Success("Permissions Retrieved", permissionDTOs, "User permissions retrieved successfully.");
        }
    }
}
