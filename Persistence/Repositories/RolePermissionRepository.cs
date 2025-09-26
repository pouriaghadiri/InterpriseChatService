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
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly ApplicationDbContext _context;
        public RolePermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(RolePermission rolePermission)
        {
            await _context.RolePermissions.AddAsync(rolePermission);
        }

        public async Task<bool> ExistsAsync(Expression<Func<RolePermission, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.RolePermissions.AnyAsync(predicate, cancellationToken);
        }

        public async Task<RolePermission?> GetbyIdAsync(Guid id)
        {
            return await _context.RolePermissions.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
