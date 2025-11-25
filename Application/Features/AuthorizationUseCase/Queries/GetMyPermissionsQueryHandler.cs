using Application.Common;
using Application.Features.AuthorizationUseCase.DTOs;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.AuthorizationUseCase.Queries
{
    public class GetMyPermissionsQueryHandler : IRequestHandler<GetMyPermissionsQuery, ResultDTO<List<PermissionDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActiveDepartmentService _activeDepartmentService;

        public GetMyPermissionsQueryHandler(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IActiveDepartmentService activeDepartmentService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _activeDepartmentService = activeDepartmentService;
        }

        public async Task<ResultDTO<List<PermissionDTO>>> Handle(GetMyPermissionsQuery request, CancellationToken cancellationToken)
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

            // Get user permissions
            var userPermissions = await _unitOfWork.UserPermissions.GetUserPermissionsAsync(userId.Value, departmentId, cancellationToken);
            if (userPermissions == null || !userPermissions.Any())
            {
                return ResultDTO<List<PermissionDTO>>.Success("No Permissions", new List<PermissionDTO>(), "You have no assigned permissions.");
            }

            var permissionDTOs = userPermissions.Select(s => s.Permission)
                                                .Select(p => new PermissionDTO
                                                {
                                                    Id = p.Id,
                                                    Name = p.Name.Value,
                                                    Description = p.Description
                                                }).ToList();

            return ResultDTO<List<PermissionDTO>>.Success("Permissions Retrieved", permissionDTOs, "Your permissions retrieved successfully.");
        }
    }
}

