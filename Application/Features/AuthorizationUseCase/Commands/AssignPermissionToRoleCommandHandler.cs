using Domain.Base;
using Domain.Base.Interface;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Commands
{
    public class AssignPermissionToRoleCommandHandler : IRequestHandler<AssignPermissionToRoleCommand, MessageDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AssignPermissionToRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<MessageDTO> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            var roleExist = await _unitOfWork.Roles.ExistsAsync(x => x.Id == request.RoleId, cancellationToken);
            if (!roleExist)
            {
                return MessageDTO.Failure("Not Exist Error", null, "The selected role doesn't have exist!");
            }
            var permissionExist = await _unitOfWork.Permissions.ExistsAsync(x => x.Id == request.PermissionId, cancellationToken);
            if (!permissionExist)
            {
                return MessageDTO.Failure("Not Exist Error", null, "The selected permission doesn't have exist!");
            }
            var departmentExist = await _unitOfWork.Departments.ExistsAsync(x => x.Id == request.DepartmentId, cancellationToken);
            if (!departmentExist)
            {
                return MessageDTO.Failure("Not Exist Error", null, "The selected department doesn't have exist!");
            }

            // Check if the permission is already assigned to the role
            var alreadyAssigned = await _unitOfWork.RolePermissions.ExistsAsync(x => x.RoleId == request.RoleId && x.PermissionId == request.PermissionId && x.DepartmentId == request.DepartmentId, cancellationToken);
            if (alreadyAssigned)
            {
                return MessageDTO.Failure("Exist Error", null, "This permission is already assigned to the role.");
            }

            var rolePermission = new RolePermission
            {
                RoleId = request.RoleId,
                PermissionId = request.PermissionId,
                DepartmentId = request.DepartmentId
            };
            await _unitOfWork.RolePermissions.AddAsync(rolePermission);
            await _unitOfWork.SaveChangesAsync();
            return MessageDTO.Success("Assigned", "Permission assigned to role successfully.");
        }
    }
}
