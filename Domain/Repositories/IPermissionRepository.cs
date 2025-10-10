using Domain.Common.ValueObjects;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IPermissionRepository
    {
        Task AddAsync(Permission permission);
        Task<Permission?> GetbyIdAsync(Guid id);
        Task<List<Permission>> GetAllAsync();
        Task UpdateAsync(Permission permission);
        Task DeleteAsync(Permission permission);
        Task<bool> ExistsAsync(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> IsPermissionInUseAsync(Guid permissionId, CancellationToken cancellationToken = default);
    }
}
