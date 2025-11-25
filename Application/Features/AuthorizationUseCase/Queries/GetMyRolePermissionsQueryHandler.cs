using Application.Common;
using Application.Features.AuthorizationUseCase.DTOs;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.AuthorizationUseCase.Queries
{
    public class GetMyRolePermissionsQueryHandler : IRequestHandler<GetMyRolePermissionsQuery, ResultDTO<List<PermissionDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActiveDepartmentService _activeDepartmentService;
        private readonly IUserRoleInDepartmentRepository _userRoleInDepartmentRepository;

        public GetMyRolePermissionsQueryHandler(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IActiveDepartmentService activeDepartmentService,
            IUserRoleInDepartmentRepository userRoleInDepartmentRepository)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _activeDepartmentService = activeDepartmentService;
            _userRoleInDepartmentRepository = userRoleInDepartmentRepository;
        }

        public async Task<ResultDTO<List<PermissionDTO>>> Handle(GetMyRolePermissionsQuery request, CancellationToken cancellationToken)
        {
            // Get current user ID from JWT token
            var userId = _httpContextAccessor.HttpContext?.User?.GetUserId();
            if (userId == null)
            {
                return ResultDTO<List<PermissionDTO>>.Failure("Unauthorized", new List<string> { "Invalid user token." }, "Unable to identify user from token.");
            }

            // Use provided departmentId or get active department
            Guid departmentId = request.DepartmentId;
            if (departmentId == Guid.Empty)
            {
                var activeDepartmentId = await _activeDepartmentService.GetActiveDepartmentIdAsync(userId.Value);
                if (activeDepartmentId == null)
                {
                    return ResultDTO<List<PermissionDTO>>.Failure("No Active Department", new List<string> { "User has no active department." }, "Please select an active department.");
                }
                departmentId = activeDepartmentId.Value;
            }

            // Verify user exists
            var userExist = await _unitOfWork.Users.ExistsAsync(x => x.Id == userId.Value, cancellationToken);
            if (!userExist)
            {
                return ResultDTO<List<PermissionDTO>>.Failure("Not Exist Error", new List<string> { "The user doesn't exist!" }, "Please contact administrator.");
            }

            // Get user's roles in the department
            var userRoles = await _userRoleInDepartmentRepository.GetRolesOfUserInDepartment(userId.Value, departmentId);
            if (userRoles == null || !userRoles.Any())
            {
                return ResultDTO<List<PermissionDTO>>.Success("No Roles", new List<PermissionDTO>(), "You have no assigned roles in this department.");
            }

            // Get permissions for all user's roles
            var roleIds = userRoles.Select(r => r.Id).ToList();
            var allRolePermissions = new List<RolePermission>();
            
            // Query permissions for each role
            foreach (var roleId in roleIds)
            {
                var rolePermissions = await _unitOfWork.RolePermissions.GetRolePermissionsAsync(roleId, departmentId, cancellationToken);
                if (rolePermissions != null && rolePermissions.Any())
                {
                    allRolePermissions.AddRange(rolePermissions);
                }
            }
            
            if (!allRolePermissions.Any())
            {
                return ResultDTO<List<PermissionDTO>>.Success("No Permissions", new List<PermissionDTO>(), "Your roles have no assigned permissions.");
            }

            // Convert to DTOs (remove duplicates)
            var permissionDTOs = allRolePermissions
                .Select(rp => rp.Permission)
                .DistinctBy(p => p.Id)
                .Select(p => new PermissionDTO
                {
                    Id = p.Id,
                    Name = p.Name.Value,
                    Description = p.Description
                }).ToList();

            return ResultDTO<List<PermissionDTO>>.Success("Permissions Retrieved", permissionDTOs, "Your role permissions retrieved successfully.");
        }
    }
}

