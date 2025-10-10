using Domain.Common.ValueObjects;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IDepartmentRepository
    {
        Task AddAsync(Department department);
        Task<Department?> GetbyIdAsync(Guid id);
        Task<List<Department>> GetAllAsync();
        Task UpdateAsync(Department department);
        Task DeleteAsync(Department department);
        Task<bool> ExistsAsync(Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> IsDepartmentInUseAsync(Guid departmentId, CancellationToken cancellationToken = default);
    }
}
