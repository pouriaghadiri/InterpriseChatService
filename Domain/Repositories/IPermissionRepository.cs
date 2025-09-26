using Domain.Common.ValueObjects;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IPermissionRepository
    {
        Task AddAsync(Permission permission);
        Task<Permission?> GetbyIdAsync(Guid id);
        Task<bool> ExistsAsync(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
