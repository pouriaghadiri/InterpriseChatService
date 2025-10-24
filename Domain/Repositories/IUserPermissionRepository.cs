using Domain.Common.ValueObjects;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IUserPermissionRepository
    {
        Task AddAsync(UserPermission userPermission);
        Task<UserPermission?> GetbyIdAsync(Guid id);
        Task<bool> ExistsAsync(Expression<Func<UserPermission, bool>> predicate, CancellationToken cancellationToken = default);
        Task<List<UserPermission>> GetUserPermissionsAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default);
        Task<HashSet<string>> GetAllUserPermissionsAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default);
    }
}
