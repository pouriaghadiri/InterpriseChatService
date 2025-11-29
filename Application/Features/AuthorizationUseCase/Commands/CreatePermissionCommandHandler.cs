using Application.Common.Services;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Commands
{
    public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, MessageDTO>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAdminPermissionAssignmentService _adminPermissionAssignmentService;
        
        public CreatePermissionCommandHandler(
            IPermissionRepository permissionRepository,
            IUnitOfWork unitOfWork,
            IAdminPermissionAssignmentService adminPermissionAssignmentService)
        {
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
            _adminPermissionAssignmentService = adminPermissionAssignmentService;
        }

        public async Task<MessageDTO> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            var permissionName = EntityName.Create(request.Name);
            if (!permissionName.IsSuccess)
            {
                return MessageDTO.Failure(permissionName.Title, permissionName.Errors, permissionName.Message);
            }
            var exists = await _permissionRepository.ExistsAsync(x => x.Name.Value == permissionName.Data.Value, cancellationToken);
            if (exists)
            {
                return MessageDTO.Failure("Exists", null, "Permission with the same name already exists.");
            }
            var permission = new Domain.Entities.Permission
            {
                Name = permissionName.Data,
                Description = request.Description
            };
            await _permissionRepository.AddAsync(permission);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Automatically assign this permission to admin role for all departments
            await _adminPermissionAssignmentService.AssignPermissionToAdminForAllDepartmentsAsync(
                permission.Id, 
                cancellationToken);

            return MessageDTO.Success("Created", "Permission created successfully.");
        }
    }
}
