using Domain.Base;
using Domain.Repositories;
using MediatR;

namespace Application.Features.AuthorizationUseCase.Commands
{
    public class DeletePermissionCommandHandler : IRequestHandler<DeletePermissionCommand, MessageDTO>
    {
        private readonly IPermissionRepository _permissionRepository;

        public DeletePermissionCommandHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<MessageDTO> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
        {
            var existingPermission = await _permissionRepository.GetbyIdAsync(request.Id);
            if (existingPermission == null)
            {
                return MessageDTO.Failure("Not Found", null, "Permission not found");
            }

            var isInUse = await _permissionRepository.IsPermissionInUseAsync(request.Id, cancellationToken);
            if (isInUse)
            {
                return MessageDTO.Failure("Conflict", null, "Permission is currently assigned to roles or users and cannot be deleted");
            }

            await _permissionRepository.DeleteAsync(existingPermission);
            return MessageDTO.Success("Deleted", "Permission deleted successfully");
        }
    }
}
