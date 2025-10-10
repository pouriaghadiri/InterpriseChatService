using Domain.Base;
using MediatR;
using System;

namespace Application.Features.RoleUseCase.Commands
{
    public class DeleteRoleCommand : IRequest<MessageDTO>
    {
        public Guid Id { get; set; }

        public DeleteRoleCommand(Guid id)
        {
            Id = id;
        }
    }
}
