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
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext _context;
        public PermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Permissions.AnyAsync(predicate, cancellationToken);
        }

        public async Task<Permission?> GetbyIdAsync(Guid id)
        {
            return await _context.Permissions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Permission>> GetAllAsync()
        {
            return await _context.Permissions.ToListAsync();
        }

        public async Task UpdateAsync(Permission permission)
        {
            _context.Permissions.Update(permission);
        }

        public async Task DeleteAsync(Permission permission)
        {
            _context.Permissions.Remove(permission);
        }

        public async Task<bool> IsPermissionInUseAsync(Guid permissionId, CancellationToken cancellationToken = default)
        {
            return await _context.RolePermissions.AnyAsync(rp => rp.PermissionId == permissionId, cancellationToken) ||
                   await _context.UserPermissions.AnyAsync(up => up.PermissionId == permissionId, cancellationToken);
        }
    }
}
