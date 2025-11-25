using Application.Common;
using Application.Features.RoleUseCase.DTOs;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.RoleUseCase.Queries
{
    public class GetMyRolesQueryHandler : IRequestHandler<GetMyRolesQuery, ResultDTO<List<RoleDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRoleInDepartmentRepository _userRoleInDepartmentRepository;

        public GetMyRolesQueryHandler(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IUserRoleInDepartmentRepository userRoleInDepartmentRepository)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userRoleInDepartmentRepository = userRoleInDepartmentRepository;
        }

        public async Task<ResultDTO<List<RoleDTO>>> Handle(GetMyRolesQuery request, CancellationToken cancellationToken)
        {
            // Get current user ID from JWT token
            var userId = _httpContextAccessor.HttpContext?.User?.GetUserId();
            if (userId == null)
            {
                return ResultDTO<List<RoleDTO>>.Failure("Unauthorized", new List<string> { "Invalid user token." }, "Unable to identify user from token.");
            }

            // Verify user exists
            var userExist = await _unitOfWork.Users.ExistsAsync(x => x.Id == userId.Value, cancellationToken);
            if (!userExist)
            {
                return ResultDTO<List<RoleDTO>>.Failure("Not Exist Error", new List<string> { "The user doesn't exist!" }, "Please contact administrator.");
            }

            // Get all roles assigned to the user
            var userRoles = await _userRoleInDepartmentRepository.GetAllRolesOfUser(userId.Value);
            if (userRoles == null || !userRoles.Any())
            {
                return ResultDTO<List<RoleDTO>>.Success("No Roles", new List<RoleDTO>(), "You have no assigned roles.");
            }

            // Convert to DTOs (remove duplicates)
            var roleDTOs = userRoles
                .DistinctBy(r => r.Id)
                .Select(r => new RoleDTO
                {
                    Id = r.Id,
                    Name = r.Name.Value,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                }).ToList();

            return ResultDTO<List<RoleDTO>>.Success("Roles Retrieved", roleDTOs, "Your roles retrieved successfully.");
        }
    }
}

