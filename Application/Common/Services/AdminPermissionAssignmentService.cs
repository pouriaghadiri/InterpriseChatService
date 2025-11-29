using Domain.Base.Interface;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Services
{
    public interface IAdminPermissionAssignmentService
    {
        Task AssignAllPermissionsToAdminForDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
        Task AssignPermissionToAdminForAllDepartmentsAsync(Guid permissionId, CancellationToken cancellationToken = default);
    }

    public class AdminPermissionAssignmentService : IAdminPermissionAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminPermissionAssignmentService> _logger;
        private string? _adminRoleName;

        public AdminPermissionAssignmentService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<AdminPermissionAssignmentService> logger)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
            _adminRoleName = _configuration["SystemRoles:AdminRoleName"];
        }

        public async Task AssignAllPermissionsToAdminForDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_adminRoleName))
            {
                _logger.LogWarning("Admin role name is not configured. Skipping automatic permission assignment.");
                return;
            }

            try
            {
                // Get admin role
                var adminRole = await _unitOfWork.Roles.GetByNameAsync(_adminRoleName, cancellationToken);
                if (adminRole == null)
                {
                    _logger.LogWarning("Admin role '{AdminRoleName}' not found. Skipping automatic permission assignment.", _adminRoleName);
                    return;
                }

                // Get all permissions
                var allPermissions = await _unitOfWork.Permissions.GetAllAsync();
                if (allPermissions == null || !allPermissions.Any())
                {
                    _logger.LogInformation("No permissions found. Skipping automatic permission assignment for department {DepartmentId}.", departmentId);
                    return;
                }

                // Filter to only admin-level permissions (ending with "_Admin")
                var adminPermissions = allPermissions
                    .Where(p => p.Name.Value.EndsWith("_Admin", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!adminPermissions.Any())
                {
                    _logger.LogInformation("No admin-level permissions found. Skipping automatic permission assignment for department {DepartmentId}.", departmentId);
                    return;
                }

                // Assign each admin-level permission to admin role for this department
                var assignedCount = 0;
                foreach (var permission in adminPermissions)
                {
                    // Check if already assigned
                    var alreadyAssigned = await _unitOfWork.RolePermissions.ExistsAsync(
                        x => x.RoleId == adminRole.Id && 
                             x.PermissionId == permission.Id && 
                             x.DepartmentId == departmentId, 
                        cancellationToken);

                    if (!alreadyAssigned)
                    {
                        var rolePermission = new RolePermission
                        {
                            RoleId = adminRole.Id,
                            PermissionId = permission.Id,
                            DepartmentId = departmentId
                        };
                        await _unitOfWork.RolePermissions.AddAsync(rolePermission);
                        assignedCount++;
                    }
                }

                if (assignedCount > 0)
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Assigned {Count} permissions to admin role for department {DepartmentId}.", assignedCount, departmentId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permissions to admin role for department {DepartmentId}.", departmentId);
                // Don't throw - this is a background operation that shouldn't fail the main operation
            }
        }

        public async Task AssignPermissionToAdminForAllDepartmentsAsync(Guid permissionId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_adminRoleName))
            {
                _logger.LogWarning("Admin role name is not configured. Skipping automatic permission assignment.");
                return;
            }

            try
            {
                // Get admin role
                var adminRole = await _unitOfWork.Roles.GetByNameAsync(_adminRoleName, cancellationToken);
                if (adminRole == null)
                {
                    _logger.LogWarning("Admin role '{AdminRoleName}' not found. Skipping automatic permission assignment.", _adminRoleName);
                    return;
                }

                // Get the permission to check if it's an admin-level permission
                var permission = await _unitOfWork.Permissions.GetbyIdAsync(permissionId);
                if (permission == null)
                {
                    _logger.LogWarning("Permission {PermissionId} not found. Skipping automatic permission assignment.", permissionId);
                    return;
                }

                // Only auto-assign permissions that end with "_Admin"
                if (!permission.Name.Value.EndsWith("_Admin", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Permission '{PermissionName}' is not an admin-level permission (does not end with '_Admin'). Skipping automatic assignment.", permission.Name.Value);
                    return;
                }

                // Get all departments
                var allDepartments = await _unitOfWork.Departments.GetAllAsync();
                if (allDepartments == null || !allDepartments.Any())
                {
                    _logger.LogInformation("No departments found. Skipping automatic permission assignment for permission {PermissionId}.", permissionId);
                    return;
                }

                // Assign this admin-level permission to admin role for all departments
                var assignedCount = 0;
                foreach (var department in allDepartments)
                {
                    // Check if already assigned
                    var alreadyAssigned = await _unitOfWork.RolePermissions.ExistsAsync(
                        x => x.RoleId == adminRole.Id && 
                             x.PermissionId == permissionId && 
                             x.DepartmentId == department.Id, 
                        cancellationToken);

                    if (!alreadyAssigned)
                    {
                        var rolePermission = new RolePermission
                        {
                            RoleId = adminRole.Id,
                            PermissionId = permissionId,
                            DepartmentId = department.Id
                        };
                        await _unitOfWork.RolePermissions.AddAsync(rolePermission);
                        assignedCount++;
                    }
                }

                if (assignedCount > 0)
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Assigned permission {PermissionId} to admin role for {Count} departments.", permissionId, assignedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permission {PermissionId} to admin role for all departments.", permissionId);
                // Don't throw - this is a background operation that shouldn't fail the main operation
            }
        }
    }
}

