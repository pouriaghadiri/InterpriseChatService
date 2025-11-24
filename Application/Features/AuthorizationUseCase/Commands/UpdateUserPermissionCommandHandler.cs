using Domain.Base;
using Domain.Base.Interface;
using Domain.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Commands
{
    public class UpdateUserPermissionCommandHandler : IRequestHandler<UpdateUserPermissionCommand, MessageDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheInvalidationService _cacheInvalidationService;

        public UpdateUserPermissionCommandHandler(IUnitOfWork unitOfWork, ICacheInvalidationService cacheInvalidationService)
        {
            _unitOfWork = unitOfWork;
            _cacheInvalidationService = cacheInvalidationService;
        }

        public async Task<MessageDTO> Handle(UpdateUserPermissionCommand request, CancellationToken cancellationToken)
        {
            // Validate input
            if (request.Id == Guid.Empty)
            {
                return MessageDTO.Failure("Invalid Input", null, "UserPermission ID is required");
            }

            // Check if at least one field is being updated
            if (!request.UserId.HasValue && !request.DepartmentId.HasValue && !request.PermissionId.HasValue)
            {
                return MessageDTO.Failure("Invalid Input", null, "At least one field (UserId, DepartmentId, or PermissionId) must be provided for update");
            }

            // Get the existing UserPermission record
            var existingUserPermission = await _unitOfWork.UserPermissions.GetbyIdAsync(request.Id);
            if (existingUserPermission == null)
            {
                return MessageDTO.Failure("Not Found", null, "UserPermission not found");
            }

            // Determine the new values (use existing if not provided)
            var newUserId = request.UserId ?? existingUserPermission.UserId;
            var newDepartmentId = request.DepartmentId ?? existingUserPermission.DepartmentId;
            var newPermissionId = request.PermissionId ?? existingUserPermission.PermissionId;

            // Validate that the new user exists (if being updated)
            if (request.UserId.HasValue)
            {
                var userExists = await _unitOfWork.Users.ExistsAsync(x => x.Id == newUserId, cancellationToken);
                if (!userExists)
                {
                    return MessageDTO.Failure("Not Exist Error", null, "The selected user doesn't exist");
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
            var alreadyExists = await _unitOfWork.UserPermissions.ExistsAsync(
                x => x.UserId == newUserId 
                     && x.DepartmentId == newDepartmentId 
                     && x.PermissionId == newPermissionId 
                     && x.Id != request.Id, 
                cancellationToken);
            
            if (alreadyExists)
            {
                return MessageDTO.Failure("Exist Error", null, "This user permission combination already exists");
            }

            // Store old values for cache invalidation
            var oldUserId = existingUserPermission.UserId;
            var oldDepartmentId = existingUserPermission.DepartmentId;

            // Update the fields
            if (request.UserId.HasValue)
            {
                existingUserPermission.UserId = newUserId;
            }
            if (request.DepartmentId.HasValue)
            {
                existingUserPermission.DepartmentId = newDepartmentId;
            }
            if (request.PermissionId.HasValue)
            {
                existingUserPermission.PermissionId = newPermissionId;
            }

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate cache for both old and new combinations
            await _cacheInvalidationService.InvalidateUserPermissionCacheAsync(oldUserId, oldDepartmentId);
            await _cacheInvalidationService.InvalidateUserDepartmentCacheAsync(oldUserId, oldDepartmentId);
            await _cacheInvalidationService.InvalidateDepartmentPermissionCacheAsync(oldDepartmentId);
            
            if (newUserId != oldUserId || newDepartmentId != oldDepartmentId)
            {
                await _cacheInvalidationService.InvalidateUserPermissionCacheAsync(newUserId, newDepartmentId);
                await _cacheInvalidationService.InvalidateUserDepartmentCacheAsync(newUserId, newDepartmentId);
            }
            if (newDepartmentId != oldDepartmentId)
            {
                await _cacheInvalidationService.InvalidateDepartmentPermissionCacheAsync(newDepartmentId);
            }
            
            return MessageDTO.Success("Updated", "User permission updated successfully");
        }
    }

   
}