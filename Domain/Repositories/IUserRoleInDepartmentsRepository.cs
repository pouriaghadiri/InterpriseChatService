using Domain.Common.ValueObjects;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IUserRoleInDepartmentRepository
    {
        Task AddAsync(UserRoleInDepartment userRoleInDepartment);
        Task<UserRoleInDepartment?> GetbyIdAsync(Guid id);
        Task<bool> ExistsAsync(Expression<Func<UserRoleInDepartment, bool>> predicate, CancellationToken cancellationToken = default);
        Task<List<Role>> GetRolesOfUserInDepartment(Guid userId, Guid departmentId);
    }
}
