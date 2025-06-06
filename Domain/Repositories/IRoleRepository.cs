using Domain.Common.ValueObjects;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IRoleRepository
    {
        Task AddAsync(Role role);
        Task<Role?> GetbyIdAsync(Guid id);
        Task<bool> ExistsAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
