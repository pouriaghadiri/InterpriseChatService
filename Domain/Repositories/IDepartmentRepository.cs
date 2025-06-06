using Domain.Common.ValueObjects;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IDepartmentRepository
    {
        Task AddAsync(Department department);
        Task<Department?> GetbyIdAsync(Guid id);
        Task<bool> ExistsAsync(Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken = default);

    }
}
