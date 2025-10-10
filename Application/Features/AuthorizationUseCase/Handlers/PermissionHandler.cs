using Application.Features.AuthorizationUseCase.Requirement;
using Application.Features.AuthorizationUseCase.Services;
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

        public PermissionHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
                throw new UnauthorizedAccessException("Invalid user id in token.");
            
            var departmentClaim = context.User.FindFirst("DepartmentId");
            if (departmentClaim == null)
                throw new UnauthorizedAccessException("Users department Couldn't find.");
            if (!Guid.TryParse(departmentClaim.Value, out var departmentId))
                throw new UnauthorizedAccessException("Invalid department id in token.");

            var hasPermission = await _permissionService.UserHasPermissionAsync(userId, departmentId, requirement.Permission);

            if (hasPermission)
                context.Succeed(requirement);
        }
    }
}
