using Domain.Common.ValueObjects;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IRolePermissionRepository
    {
        Task AddAsync(RolePermission rolePermission);
        Task<RolePermission?> GetbyIdAsync(Guid id);
        Task<bool> ExistsAsync(Expression<Func<RolePermission, bool>> predicate, CancellationToken cancellationToken = default);
        Task<List<RolePermission>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
        //Task<bool> HasUserPermissionAsync(Guid userId, Guid departmentId, string permissionName);

    }
}
