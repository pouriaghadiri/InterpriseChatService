using Domain.Base;
using Domain.Base.Interface;
using Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.RoleUseCase.Commands
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, MessageDTO>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoleCommandHandler(IUnitOfWork unitOfWork, IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MessageDTO> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var existingRole = await _roleRepository.GetbyIdAsync(request.Id);
            if (existingRole == null)
            {
                return MessageDTO.Failure("Not Found", null, "Role not found.");
            }

            // Check if another role with the same name exists (excluding current role)
            var roleWithSameName = await _roleRepository.ExistsAsync(x => x.Name == request.Name && x.Id != request.Id, cancellationToken);
            if (roleWithSameName)
            {
                return MessageDTO.Failure("Exist Error", null, "A role with this name already exists.");
            }

            var roleName = Domain.Common.ValueObjects.EntityName.Create(request.Name?.Value);
            if (!roleName.IsSuccess)
            {
                return MessageDTO.Failure(roleName.Title, roleName.Errors, roleName.Message);
            }

            // Update the role properties
            existingRole.Name = roleName.Data;
            existingRole.Description = request.Description;

            await _roleRepository.UpdateAsync(existingRole);
            await _unitOfWork.SaveChangesAsync();

            return MessageDTO.Success("Updated", "Role updated successfully.");
        }
    }
}
