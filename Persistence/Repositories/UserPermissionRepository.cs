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
    }
}
