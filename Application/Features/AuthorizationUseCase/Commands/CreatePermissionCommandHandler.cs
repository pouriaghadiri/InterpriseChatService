using Domain.Base;
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
        public CreatePermissionCommandHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
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
            return MessageDTO.Success("Created", "Permission created successfully.");
        }
    }
}
