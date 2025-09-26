using Domain.Base;
using Domain.Base.Interface;
using Domain.Entities;
using MediatR;

namespace Application.Features.AuthorizationUseCase.Commands
{
    class AssignPermissionToUserCommandHandler : IRequestHandler<AssignPermissionToUserCommand, MessageDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AssignPermissionToUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<MessageDTO> Handle(AssignPermissionToUserCommand request, CancellationToken cancellationToken)
        {
            var userExist = await _unitOfWork.Users.ExistsAsync(x => x.Id == request.UserId, cancellationToken);
            if (!userExist)
            {
                return MessageDTO.Failure("Not Exist Error", null, "The selected user doesn't have exist!");
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
            var alreadyAssigned = await _unitOfWork.UserPermissions.ExistsAsync(x => x.UserId == request.UserId && x.PermissionId == request.PermissionId && x.DepartmentId == request.DepartmentId, cancellationToken);
            if (alreadyAssigned)
            {
                return MessageDTO.Failure("Exist Error", null, "This permission is already assigned to the user.");
            }

            var userPermission = new UserPermission
            {
                UserId = request.UserId,
                PermissionId = request.PermissionId,
                DepartmentId = request.DepartmentId
            };
            await _unitOfWork.UserPermissions.AddAsync(userPermission);
            await _unitOfWork.SaveChangesAsync();
            return MessageDTO.Success("Assigned", "Permission assigned to user successfully.");
        }
    }
}
