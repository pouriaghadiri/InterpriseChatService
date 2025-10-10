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
        public async Task<List<RolePermission>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _context.RolePermissions
                .Include(up => up.Permission)
                .Where(up => up.RoleId == roleId)
                .ToListAsync(cancellationToken);
        }
        //public async Task<bool> HasUserPermissionAsync(Guid userId, Guid departmentId, string permissionName)
        //{
        //    var createName = EntityName.Create(permissionName);
        //    if (!createName.IsSuccess)
        //    {
        //        return false;
        //    }
        //    return await (from rp in _context.RolePermissions
        //                  join ur in _context.UserRoles on rp.RoleId equals ur.RoleId
        //                  where ur.UserId == userId
        //                        && rp.Permission.Name == createName.Data
        //                  select rp).AnyAsync();
        //}
    }
}
