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
    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly ApplicationDbContext _context;
        public UserPermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(UserPermission userPermission)
        {
            await _context.UserPermissions.AddAsync(userPermission);
        }

        public async Task<bool> ExistsAsync(Expression<Func<UserPermission, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.UserPermissions.AnyAsync(predicate, cancellationToken);
        }

        public async Task<UserPermission?> GetbyIdAsync(Guid id)
        {
            return await _context.UserPermissions.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<UserPermission>> GetUserPermissionsAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default)
        {
            return await _context.UserPermissions
                .Include(up => up.Permission)
                .Where(up => up.UserId == userId && up.DepartmentId == departmentId)
                .ToListAsync(cancellationToken);
        }

        public async Task<HashSet<string>> GetAllUserPermissionsAsync(Guid userId, Guid departmentId, CancellationToken cancellationToken = default)
        {
            var permissions = new HashSet<string>();

            // Get direct user permissions
            var userPermissions = await _context.UserPermissions
                .Include(up => up.Permission)
                .Where(up => up.UserId == userId && up.DepartmentId == departmentId)
                .Select(up => up.Permission.Name.Value)
                .ToListAsync(cancellationToken);

            permissions.UnionWith(userPermissions);

            // Get role permissions
            var userRoles = await _context.UserRoleInDepartments
                .Where(urid => urid.UserRole.UserId == userId && urid.DepartmentId == departmentId)
                .Select(urid => urid.UserRole.RoleId)
                .ToListAsync(cancellationToken);

            if (userRoles.Any())
            {
                var rolePermissions = await _context.RolePermissions
                    .Include(rp => rp.Permission)
                    .Where(rp => userRoles.Contains(rp.RoleId) && rp.DepartmentId == departmentId)
                    .Select(rp => rp.Permission.Name.Value)
                    .ToListAsync(cancellationToken);

                permissions.UnionWith(rolePermissions);
            }

            return permissions;
        }
    }
}
