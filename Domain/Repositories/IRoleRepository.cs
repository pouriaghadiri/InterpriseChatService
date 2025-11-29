using Domain.Common.ValueObjects;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IRoleRepository
    {
        Task AddAsync(Role role);
        Task<Role?> GetbyIdAsync(Guid id);
        Task<List<Role>> GetAllAsync();
        Task UpdateAsync(Role role);
        Task DeleteAsync(Role role);
        Task<bool> ExistsAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> IsRoleInUseAsync(Guid roleId, CancellationToken cancellationToken = default);
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
