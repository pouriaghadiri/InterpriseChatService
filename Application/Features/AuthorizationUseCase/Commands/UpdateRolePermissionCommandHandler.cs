using Domain.Base;
using Domain.Base.Interface;
using Domain.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Commands
{
    public class UpdateRolePermissionCommandHandler : IRequestHandler<UpdateRolePermissionCommand, MessageDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheInvalidationService _cacheInvalidationService;

        public UpdateRolePermissionCommandHandler(IUnitOfWork unitOfWork, ICacheInvalidationService cacheInvalidationService)
        {
            _unitOfWork = unitOfWork;
            _cacheInvalidationService = cacheInvalidationService;
        }

        public async Task<MessageDTO> Handle(UpdateRolePermissionCommand request, CancellationToken cancellationToken)
        {
            // Validate input
            if (request.Id == Guid.Empty)
            {
                return MessageDTO.Failure("Invalid Input", null, "RolePermission ID is required");
            }

            // Check if at least one field is being updated
            if (!request.RoleId.HasValue && !request.DepartmentId.HasValue && !request.PermissionId.HasValue)
            {
                return MessageDTO.Failure("Invalid Input", null, "At least one field (RoleId, DepartmentId, or PermissionId) must be provided for update");
            }

            // Get the existing RolePermission record
            var existingRolePermission = await _unitOfWork.RolePermissions.GetbyIdAsync(request.Id);
            if (existingRolePermission == null)
            {
                return MessageDTO.Failure("Not Found", null, "RolePermission not found");
            }

            // Determine the new values (use existing if not provided)
            var newRoleId = request.RoleId ?? existingRolePermission.RoleId;
            var newDepartmentId = request.DepartmentId ?? existingRolePermission.DepartmentId;
            var newPermissionId = request.PermissionId ?? existingRolePermission.PermissionId;

            // Validate that the new role exists (if being updated)
            if (request.RoleId.HasValue)
            {
                var roleExists = await _unitOfWork.Roles.ExistsAsync(x => x.Id == newRoleId, cancellationToken);
                if (!roleExists)
                {
                    return MessageDTO.Failure("Not Exist Error", null, "The selected role doesn't exist");
                }
            }

            // Validate that the new department exists (if being updated)
            if (request.DepartmentId.HasValue)
            {
                var departmentExists = await _unitOfWork.Departments.ExistsAsync(x => x.Id == newDepartmentId, cancellationToken);
                if (!departmentExists)
                {
                    return MessageDTO.Failure("Not Exist Error", null, "The selected department doesn't exist");
                }
            }

            // Validate that the new permission exists (if being updated)
            if (request.PermissionId.HasValue)
            {
                var permissionExists = await _unitOfWork.Permissions.ExistsAsync(x => x.Id == newPermissionId, cancellationToken);
                if (!permissionExists)
                {
                    return MessageDTO.Failure("Not Exist Error", null, "The selected permission doesn't exist");
                }
            }

            // Check if the same combination already exists (avoid duplicates)
            var alreadyExists = await _unitOfWork.RolePermissions.ExistsAsync(
                x => x.RoleId == newRoleId 
                     && x.DepartmentId == newDepartmentId 
                     && x.PermissionId == newPermissionId 
                     && x.Id != request.Id, 
                cancellationToken);
            
            if (alreadyExists)
            {
                return MessageDTO.Failure("Exist Error", null, "This role permission combination already exists");
            }

            // Store old values for cache invalidation
            var oldRoleId = existingRolePermission.RoleId;
            var oldDepartmentId = existingRolePermission.DepartmentId;

            // Update the fields
            if (request.RoleId.HasValue)
            {
                existingRolePermission.RoleId = newRoleId;
            }
            if (request.DepartmentId.HasValue)
            {
                existingRolePermission.DepartmentId = newDepartmentId;
            }
            if (request.PermissionId.HasValue)
            {
                existingRolePermission.PermissionId = newPermissionId;
            }

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate cache for both old and new combinations
            await _cacheInvalidationService.InvalidateRolePermissionCacheAsync(oldRoleId, oldDepartmentId);
            if (newRoleId != oldRoleId || newDepartmentId != oldDepartmentId)
            {
                await _cacheInvalidationService.InvalidateRolePermissionCacheAsync(newRoleId, newDepartmentId);
            }
            await _cacheInvalidationService.InvalidateDepartmentPermissionCacheAsync(oldDepartmentId);
            if (newDepartmentId != oldDepartmentId)
            {
                await _cacheInvalidationService.InvalidateDepartmentPermissionCacheAsync(newDepartmentId);
            }
            
            return MessageDTO.Success("Updated", "Role permission updated successfully");
        }
    }

    
}
