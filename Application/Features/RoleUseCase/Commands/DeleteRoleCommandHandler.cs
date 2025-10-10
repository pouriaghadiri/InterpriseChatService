using Domain.Base;
using Domain.Base.Interface;
using Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.RoleUseCase.Commands
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, MessageDTO>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRoleCommandHandler(IUnitOfWork unitOfWork, IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MessageDTO> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var existingRole = await _roleRepository.GetbyIdAsync(request.Id);
            if (existingRole == null)
            {
                return MessageDTO.Failure("Not Found", null, "Role not found.");
            }

            // Check if role is being used by any users
            var isRoleInUse = await _roleRepository.IsRoleInUseAsync(request.Id, cancellationToken);
            if (isRoleInUse)
            {
                return MessageDTO.Failure("In Use Error", null, "Cannot delete role that is assigned to users.");
            }

            await _roleRepository.DeleteAsync(existingRole);
            await _unitOfWork.SaveChangesAsync();

            return MessageDTO.Success("Deleted", "Role deleted successfully.");
        }
    }
}
