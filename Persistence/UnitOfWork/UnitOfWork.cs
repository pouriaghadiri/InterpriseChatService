
using Domain.Base.Interface;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IRoleRepository Roles { get; }
        public IPermissionRepository Permissions { get; }
        public IRolePermissionRepository RolePermissions { get; }
        public IUserPermissionRepository UserPermissions { get; }
        public IDepartmentRepository Departments { get; }
        public IUserRepository Users { get; }
        public IUserRoleInDepartmentRepository userRoleInDepartment{ get; }

        public UnitOfWork(ApplicationDbContext context,IRoleRepository roleRepository, IPermissionRepository permissionRepository,
                          IRolePermissionRepository rolePermissionRepository, IUserPermissionRepository userPermissionRepository,
                          IDepartmentRepository departmentRepository, IUserRepository userRepository,
                          IUserRoleInDepartmentRepository userRoleInDepartment)
        {
            _context = context;
            Roles = roleRepository;
            Permissions = permissionRepository;
            RolePermissions = rolePermissionRepository;
            UserPermissions = userPermissionRepository;
            Departments = departmentRepository;
            Users = userRepository;
            this.userRoleInDepartment = userRoleInDepartment;
        }



        public Task BeginTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task CommitTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task RollBackTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellation = default)
        {
            return await _context.SaveChangesAsync(cancellation);
        }
    }
}
