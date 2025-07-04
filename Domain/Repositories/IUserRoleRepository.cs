using Domain.Common.ValueObjects;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IUserRoleRepository
    {
        Task AddAsync(UserRole userRole);
        Task<UserRole?> GetbyIdAsync(Guid id);
        Task<bool> ExistsAsync(Expression<Func<UserRole, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
