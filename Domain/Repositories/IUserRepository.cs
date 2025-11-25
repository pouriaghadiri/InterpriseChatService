using Domain.Common.ValueObjects;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistsAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default);
        Task AddAsync(User user);
        Task<User?> GetbyEmailAsync(Email email);
        Task<User?> GetbyIdAsync(Guid id);
        Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);
        Task DeleteAsync(User user);
    }
}
