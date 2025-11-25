using Application.Features.UserUseCase.DTOs;
using Domain.Base;
using Domain.Base.Interface;
using MediatR;

namespace Application.Features.UserUseCase.Queries
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ResultDTO<List<UserDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDTO<List<UserDTO>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
            
            if (users == null || !users.Any())
            {
                return ResultDTO<List<UserDTO>>.Success("No Users", new List<UserDTO>(), "No users found.");
            }

            var userDTOs = users.Select(user => new UserDTO
            {
                ID = user.Id,
                FullName = user.FullName,
                UserRoleInDepartments = user.UserRoles.SelectMany(s => s.UserRoleInDepartments.Select(r => new UserRoleInDepartmentDTO
                {
                    DepartmentID = r.DepartmentId,
                    DepartmentName = r.Department.Name,
                    RoleID = r.UserRole.RoleId,
                    RoleName = r.UserRole.Role.Name,
                })).ToList(),
            }).ToList();

            return ResultDTO<List<UserDTO>>.Success("Retrieved", userDTOs, "Users retrieved successfully.");
        }
    }
}

