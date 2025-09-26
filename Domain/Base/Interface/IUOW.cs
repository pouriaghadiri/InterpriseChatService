using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base.Interface
{
    public interface IUnitOfWork
    {
        IRoleRepository Roles { get; }
        IPermissionRepository Permissions { get; }
        IRolePermissionRepository RolePermissions { get; }
        IUserPermissionRepository UserPermissions{ get; }
        IDepartmentRepository Departments { get; }
        IUserRepository Users { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellation = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollBackTransactionAsync();
    }
}
