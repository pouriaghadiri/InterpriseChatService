using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UserRoleInDepartmentRepository : IUserRoleInDepartmentRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRoleInDepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(UserRoleInDepartment UserRoleInDepartment)
        {
            await _context.UserRoleInDepartments.AddAsync(UserRoleInDepartment);
        }

        public async Task<bool> ExistsAsync(Expression<Func<UserRoleInDepartment, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.UserRoleInDepartments.AnyAsync(predicate, cancellationToken);
        }

        public async Task<UserRoleInDepartment?> GetbyIdAsync(Guid id)
        {
            return await _context.UserRoleInDepartments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Role>> GetRolesOfUserInDepartment(Guid userId, Guid departmentId)
        {
            return await _context.UserRoleInDepartments.Where(x => x.UserRole.UserId == userId &&
                                                                   x.DepartmentId == departmentId)
                                                        .Select(s => s.UserRole.Role)
                                                        .ToListAsync();
        }
    }
}
