using System;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IActiveDepartmentService
    {
        /// <summary>
        /// Get user's active department ID from cache or database
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Active department ID or null if not found</returns>
        Task<Guid?> GetActiveDepartmentIdAsync(Guid userId);

        /// <summary>
        /// Set user's active department ID in cache and database
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="departmentId">Department ID to set as active</param>
        /// <returns>Success result</returns>
        Task<bool> SetActiveDepartmentIdAsync(Guid userId, Guid departmentId);

        /// <summary>
        /// Remove user's active department ID from cache
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Success result</returns>
        Task<bool> RemoveActiveDepartmentIdAsync(Guid userId);

        /// <summary>
        /// Check if user has an active department in cache
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>True if active department exists in cache</returns>
        Task<bool> HasActiveDepartmentAsync(Guid userId);
    }
}
