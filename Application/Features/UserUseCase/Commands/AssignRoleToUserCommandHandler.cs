using Domain.Base;
using Domain.Base.Interface;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserUseCase.Commands
{
    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, MessageDTO>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserRoleInDepartmentRepository _userRoleInDepartmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AssignRoleToUserCommandHandler(IRoleRepository roleRepository, IUserRepository userRepository,
                                              IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork,
                                              IUserRoleRepository userRoleRepository, IUserRoleInDepartmentRepository userRoleInDepartmentRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
            _userRoleInDepartmentRepository = userRoleInDepartmentRepository;
        }


        public async Task<MessageDTO> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            var existUser = await _userRepository.GetbyIdAsync(request.User.Id);
            if (existUser == null)
            {
                return MessageDTO.Failure("Not Exist Error", null, "The selected user doesn't have exist!");
            }
            var existDepartment = await _departmentRepository.ExistsAsync(x => x.Id == request.Department.Id, cancellationToken);
            if (!existDepartment)
            {
                return MessageDTO.Failure("Not Exist Error", null, "The selected department doesn't have exist!");
            }
            var existRole = await _roleRepository.ExistsAsync(x => x.Id == request.Role.Id, cancellationToken);
            if (!existRole)
            {
                return MessageDTO.Failure("Not Exist Error", null, "The selected role doesn't have exist!");
            }

            var existUserRole = await _userRoleInDepartmentRepository.ExistsAsync(x => x.UserRole.UserId ==  request.User.Id &&
                                                                                 x.UserRole.RoleId == request.Role.Id && 
                                                                                 x.DepartmentId == request.Department.Id  
                                                                            , cancellationToken);
            if (existUserRole)
            {
                return MessageDTO.Failure("Exist Error", null, "This Assignment is exist!");
            }

            var newUserRole = existUser.AssignRoleToUser(request.Role, request.Department);
            if (!newUserRole.IsSuccess)
            {
                return MessageDTO.Failure(newUserRole.Title, newUserRole.Errors, newUserRole.Message);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MessageDTO.Success("Created", "Assigned role to user successfully complleted.");

        }
    }
}
