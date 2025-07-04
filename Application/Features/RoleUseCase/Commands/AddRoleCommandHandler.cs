using Application.Features.DepartmentUseCase.Commands;
using Domain.Base.Interface;
using Domain.Base;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common.ValueObjects;
using Domain.Entities;

namespace Application.Features.RoleUseCase.Commands
{
    class AddRoleCommandHandler : IRequestHandler<AddRoleCommand, MessageDTO>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddRoleCommandHandler(IUnitOfWork unitOfWork, IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MessageDTO> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            var existRole = await _roleRepository.ExistsAsync(x => x.Name == request.Name, cancellationToken);
            if (existRole)
            {
                return MessageDTO.Failure("Exist Error", null, "This role already exists!");
            }

            var roleName = EntityName.Create(request.Name?.Value);
            if (!roleName.IsSuccess)
            {
                return MessageDTO.Failure(roleName.Title, roleName.Errors, roleName.Message);
            }

            var newRole = Role.CreateRole(roleName.Data, request.Description);
            if (!newRole.IsSuccess)
            {
                return MessageDTO.Failure(newRole.Title, newRole.Errors, newRole.Message);
            }

            await _roleRepository.AddAsync(newRole.Data);
            await _unitOfWork.SaveChangesAsync();

            return MessageDTO.Success("Created", "Role added successfully.");

        }
    }
}
