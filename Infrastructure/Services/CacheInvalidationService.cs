using Application.Common;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CacheInvalidationService : ICacheInvalidationService
    {
        private readonly ICacheService _cacheService;

        public CacheInvalidationService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task InvalidateRolePermissionCacheAsync(Guid roleId, Guid? departmentId = null)
        {
            // Remove role permission cache
            var rolePermissionKey = departmentId.HasValue 
                ? CacheHelper.RolePermissionsKey(roleId, departmentId.Value)
                : $"role:{roleId}:permissions:*";

            if (departmentId.HasValue)
            {
                await _cacheService.RemoveAsync(rolePermissionKey);
            }
            else
            {
                // Remove all role permission caches for this role
                await RemoveByPatternAsync($"role:{roleId}:permissions:*");
            }

            // Remove all user permission caches that might be affected by this role
            await RemoveByPatternAsync("user:*:all-permissions:*");
        }

        public async Task InvalidateUserPermissionCacheAsync(Guid userId, Guid? departmentId = null)
        {
            if (departmentId.HasValue)
            {
                // Remove specific user permission cache
                var userPermissionKey = CacheHelper.UserAllPermissionsKey(userId, departmentId.Value);
                await _cacheService.RemoveAsync(userPermissionKey);
            }
            else
            {
                // Remove all user permission caches for this user
                await RemoveByPatternAsync($"user:{userId}:all-permissions:*");
            }
        }

        public async Task InvalidateUserCacheAsync(Guid userId, string email = null)
        {
            // Remove all user-related caches
            var cacheKeys = new List<string>
            {
                CacheHelper.UserKey(userId),
                CacheHelper.UserActiveDepartmentKey(userId),
                $"user:{userId}:all-permissions:*",
                $"user:{userId}:permissions:*",
                $"user:{userId}:roles:*",
                $"session:{userId}",
                $"token:*:{userId}" // Token caches
            };

            // Also invalidate email-based cache if email is provided
            if (!string.IsNullOrWhiteSpace(email))
            {
                cacheKeys.Add(CacheHelper.UserByEmailKey(email));
            }

            foreach (var pattern in cacheKeys)
            {
                if (pattern.Contains("*"))
                {
                    await RemoveByPatternAsync(pattern);
                }
                else
                {
                    await _cacheService.RemoveAsync(pattern);
                }
            }
        }

        public async Task InvalidateUserCacheByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            var cacheKey = CacheHelper.UserByEmailKey(email);
            await _cacheService.RemoveAsync(cacheKey);
        }

        public async Task InvalidateUserDepartmentCacheAsync(Guid userId, Guid departmentId)
        {
            var cacheKeys = new List<string>
            {
                CacheHelper.UserAllPermissionsKey(userId, departmentId),
                CacheHelper.UserPermissionsKey(userId, departmentId),
                CacheHelper.UserRoleKey(userId, departmentId)
            };

            foreach (var key in cacheKeys)
            {
                await _cacheService.RemoveAsync(key);
            }
        }

        public async Task InvalidateDepartmentPermissionCacheAsync(Guid departmentId)
        {
            // Remove all permission caches for this department
            await RemoveByPatternAsync($"*:permissions:{departmentId}");
            await RemoveByPatternAsync($"*:all-permissions:{departmentId}");
            await RemoveByPatternAsync($"*:roles:{departmentId}");
        }

        private async Task RemoveByPatternAsync(string pattern)
        {
            // Note: This is a simplified implementation
            // In production, you might want to use Redis SCAN command
            // For now, we'll implement a basic pattern matching
            try
            {
                // This is a placeholder - in real implementation you would:
                // 1. Use Redis SCAN command to find matching keys
                // 2. Remove each matching key
                // For now, we'll log the pattern that would be removed
                Console.WriteLine($"Would remove cache keys matching pattern: {pattern}");
            }
            catch (Exception ex)
            {
                // Log the exception but don't throw
                Console.WriteLine($"Error removing cache pattern {pattern}: {ex.Message}");
            }
        }
    }
}
