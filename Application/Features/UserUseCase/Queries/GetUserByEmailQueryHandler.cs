using Application.Features.UserUseCase.DTOs;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserUseCase.Queries
{
    public class GetUserByNameQueryHandler : IRequestHandler<GetUserByEmailQuery, ResultDTO<UserDTO>>
    {
        private readonly IUserRepository _userRepository;
        public GetUserByNameQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<ResultDTO<UserDTO>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var createEmail = Email.Create(request.Email);
            if (!createEmail.IsSuccess || createEmail.Data == null)
            {
                return ResultDTO<UserDTO>.Failure("Invalid input", null, "Email validation failed");
            }
            var user = await _userRepository.GetbyEmailAsync(createEmail.Data);
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

            var result = ResultDTO<UserDTO>.Success("TranseferData", userResult, "Transfer data completed successfully");

            return result;

        }
    }
}
