using Domain.Common.ValueObjects;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IUserRoleInDepartmentRepository
    {
        Task AddAsync(UserRoleInDepartment userRoleInDepartment);
        Task<UserRoleInDepartment?> GetbyIdAsync(Guid id);
        Task<bool> ExistsAsync(Expression<Func<UserRoleInDepartment, bool>> predicate, CancellationToken cancellationToken = default);
        Task<List<Role>> GetRolesOfUserInDepartment(Guid userId, Guid departmentId);
        /// <summary>
        /// Get all roles assigned to a user across all departments
        /// </summary>
        Task<List<Role>> GetAllRolesOfUser(Guid userId);
        /// <summary>
        /// Get all departments where a user has roles assigned
        /// </summary>
        Task<List<Department>> GetAllDepartmentsOfUser(Guid userId);
    }
}
