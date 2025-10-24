using Application.Common;
using Application.Features.AuthorizationUseCase.Services;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Repositories;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pplication.Features.AuthorizationUseCase.Services
{

    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public PermissionService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<bool> UserHasPermissionAsync(Guid userId, Guid departmentId, string permissionName)
        {
            var createPermissionName = EntityName.Create(permissionName);
            if (!createPermissionName.IsSuccess || createPermissionName?.Data == null)
            {
                return false;
            }

            var cacheKey = CacheHelper.UserAllPermissionsKey(userId, departmentId);
            var cachedPermissions = await _cacheService.GetAsync<HashSet<string>>(cacheKey);
            
            if (cachedPermissions == null)
            {
                // Get all permissions (user + role permissions) from database
                var allPermissions = await _unitOfWork.UserPermissions.GetAllUserPermissionsAsync(userId, departmentId);
                
                // Cache the result
                await _cacheService.SetAsync(cacheKey, allPermissions, CacheHelper.Expiration.Permission);
                
                return allPermissions.Contains(createPermissionName.Data.Value);
            }
            
            // Use cached permissions
            return cachedPermissions.Contains(createPermissionName.Data.Value);
        }
    }
}

