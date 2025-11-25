using Application.Features.UserUseCase.DTOs;
using Domain.Base;
using Domain.Repositories;
using MediatR;

namespace Application.Features.UserUseCase.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ResultDTO<UserDTO>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResultDTO<UserDTO>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                return ResultDTO<UserDTO>.Failure("Invalid Input", null, "User ID is required.");
            }

            var user = await _userRepository.GetbyIdAsync(request.Id);
            if (user == null)
            {
                return ResultDTO<UserDTO>.Failure("NotFound Error", null, "User is not found!");
            }

            var userResult = new UserDTO
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
            };

            return ResultDTO<UserDTO>.Success("Retrieved", userResult, "User retrieved successfully.");
        }
    }
}

