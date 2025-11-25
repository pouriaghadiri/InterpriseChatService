using Application.Common;
using Application.Features.DepartmentUseCase.DTOs;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.DepartmentUseCase.Queries
{
    public class GetMyDepartmentsQueryHandler : IRequestHandler<GetMyDepartmentsQuery, ResultDTO<List<DepartmentDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRoleInDepartmentRepository _userRoleInDepartmentRepository;

        public GetMyDepartmentsQueryHandler(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IUserRoleInDepartmentRepository userRoleInDepartmentRepository)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userRoleInDepartmentRepository = userRoleInDepartmentRepository;
        }

        public async Task<ResultDTO<List<DepartmentDTO>>> Handle(GetMyDepartmentsQuery request, CancellationToken cancellationToken)
        {
            // Get current user ID from JWT token
            var userId = _httpContextAccessor.HttpContext?.User?.GetUserId();
            if (userId == null)
            {
                return ResultDTO<List<DepartmentDTO>>.Failure("Unauthorized", new List<string> { "Invalid user token." }, "Unable to identify user from token.");
            }

            // Verify user exists
            var userExist = await _unitOfWork.Users.ExistsAsync(x => x.Id == userId.Value, cancellationToken);
            if (!userExist)
            {
                return ResultDTO<List<DepartmentDTO>>.Failure("Not Exist Error", new List<string> { "The user doesn't exist!" }, "Please contact administrator.");
            }

            // Get all departments where the user has roles
            var userDepartments = await _userRoleInDepartmentRepository.GetAllDepartmentsOfUser(userId.Value);
            if (userDepartments == null || !userDepartments.Any())
            {
                return ResultDTO<List<DepartmentDTO>>.Success("No Departments", new List<DepartmentDTO>(), "You are not assigned to any departments.");
            }

            // Convert to DTOs (remove duplicates)
            var departmentDTOs = userDepartments
                .DistinctBy(d => d.Id)
                .Select(d => new DepartmentDTO
                {
                    Id = d.Id,
                    Name = d.Name.Value,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                }).ToList();

            return ResultDTO<List<DepartmentDTO>>.Success("Departments Retrieved", departmentDTOs, "Your departments retrieved successfully.");
        }
    }
}

