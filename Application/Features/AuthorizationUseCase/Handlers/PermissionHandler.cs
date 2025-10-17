using Application.Features.AuthorizationUseCase.Requirement;
using Application.Features.AuthorizationUseCase.Services;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Handlers
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;
        private readonly IActiveDepartmentService _activeDepartmentService;

        public PermissionHandler(IPermissionService permissionService, IActiveDepartmentService activeDepartmentService)
        {
            _permissionService = permissionService;
            _activeDepartmentService = activeDepartmentService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
                throw new UnauthorizedAccessException("Invalid user id in token.");
            
            // Get active department from Redis cache (with database fallback)
            var activeDepartmentId = await _activeDepartmentService.GetActiveDepartmentIdAsync(userId);
            
            if (activeDepartmentId == null)
                throw new UnauthorizedAccessException("User has no active department. Please contact administrator.");

            var hasPermission = await _permissionService.UserHasPermissionAsync(userId, activeDepartmentId.Value, requirement.Permission);

            if (hasPermission)
                context.Succeed(requirement);
        }
    }
}
