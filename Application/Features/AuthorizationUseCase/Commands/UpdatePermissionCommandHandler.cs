using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Repositories;
using MediatR;

namespace Application.Features.AuthorizationUseCase.Commands
{
    public class UpdatePermissionCommandHandler : IRequestHandler<UpdatePermissionCommand, MessageDTO>
    {
        private readonly IPermissionRepository _permissionRepository;

        public UpdatePermissionCommandHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<MessageDTO> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        {
            var existingPermission = await _permissionRepository.GetbyIdAsync(request.Id);
            if (existingPermission == null)
            {
                return MessageDTO.Failure("Not Found", null, "Permission not found");
            }

            var permissionName = EntityName.Create(request.Name);
            if (!permissionName.IsSuccess)
            {
                return MessageDTO.Failure(permissionName.Title, permissionName.Errors, permissionName.Message);
            }

            var permissionWithSameName = await _permissionRepository.ExistsAsync(x => x.Name == permissionName.Data && x.Id != request.Id, cancellationToken);
            if (permissionWithSameName)
            {
                return MessageDTO.Failure("Conflict", null, "Permission with the same name already exists");
            }

            existingPermission.Name = permissionName.Data;
            existingPermission.Description = request.Description;

            await _permissionRepository.UpdateAsync(existingPermission);
            return MessageDTO.Success("Updated", "Permission updated successfully");
        }
    }
}
