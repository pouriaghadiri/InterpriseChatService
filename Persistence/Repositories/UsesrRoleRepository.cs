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
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(UserRole UserRole)
        {
            await _context.UserRoles.AddAsync(UserRole);
        }

        public async Task<bool> ExistsAsync(Expression<Func<UserRole, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.UserRoles.AnyAsync(predicate, cancellationToken);
        }

        public async Task<UserRole?> GetbyIdAsync(Guid id)
        {
            return await _context.UserRoles.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
