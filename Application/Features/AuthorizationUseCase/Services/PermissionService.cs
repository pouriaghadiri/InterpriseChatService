using Application.Features.AuthorizationUseCase.Services;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pplication.Features.AuthorizationUseCase.Services
{

    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PermissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> UserHasPermissionAsync(Guid userId, Guid departmentId, string permissionName)
        {
            var createPermissionName = EntityName.Create(permissionName);
            if (!createPermissionName.IsSuccess || createPermissionName?.Data == null)
            {
                return false;
            }
            var userHas = await _unitOfWork.UserPermissions.ExistsAsync(
                up => up.UserId == userId && 
                      up.Permission.Name.Value == createPermissionName.Data.Value && 
                      up.DepartmentId == departmentId);

            if (userHas) return true;

            var userRoles = await _unitOfWork.userRoleInDepartment.GetRolesOfUserInDepartment(userId, departmentId);
            if (userRoles == null || !userRoles.Any())
            {
                return false;
            }
            var roleHas = await _unitOfWork.RolePermissions.ExistsAsync(
                rp => userRoles.Select(s => s.Id).Contains(rp.RoleId) &&
                      rp.Permission.Name.Value == createPermissionName.Data.Value &&
                      rp.DepartmentId == departmentId);

            return roleHas;
        }
    }
}

