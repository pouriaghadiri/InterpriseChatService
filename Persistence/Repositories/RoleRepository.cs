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
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Roles.AnyAsync(predicate, cancellationToken);
        }

        public async Task<Role?> GetbyIdAsync(Guid id)
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
