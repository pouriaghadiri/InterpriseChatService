using System;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ICacheInvalidationService
    {
        /// <summary>
        /// Remove all user permission caches when role permissions change
        /// </summary>
        /// <param name="roleId">Role ID that was updated</param>
        /// <param name="departmentId">Department ID (optional)</param>
        Task InvalidateRolePermissionCacheAsync(Guid roleId, Guid? departmentId = null);

        /// <summary>
        /// Remove user permission cache when user permissions change
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="departmentId">Department ID (optional)</param>
        Task InvalidateUserPermissionCacheAsync(Guid userId, Guid? departmentId = null);

        /// <summary>
        /// Remove all user caches when user logs out or user data is updated
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="email">User email (optional) - if provided, also invalidates email-based cache</param>
        Task InvalidateUserCacheAsync(Guid userId, string email = null);

        /// <summary>
        /// Remove user cache by email (used when user profile is updated)
        /// </summary>
        /// <param name="email">User email</param>
        Task InvalidateUserCacheByEmailAsync(string email);

        /// <summary>
        /// Remove all caches for a specific user and department
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="departmentId">Department ID</param>
        Task InvalidateUserDepartmentCacheAsync(Guid userId, Guid departmentId);

        /// <summary>
        /// Remove all permission caches for a department
        /// </summary>
        /// <param name="departmentId">Department ID</param>
        Task InvalidateDepartmentPermissionCacheAsync(Guid departmentId);
    }
}
