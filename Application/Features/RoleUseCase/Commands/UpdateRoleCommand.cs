using Domain.Base;
using Domain.Common.ValueObjects;
using MediatR;
using System;

namespace Application.Features.RoleUseCase.Commands
{
    public class UpdateRoleCommand : IRequest<MessageDTO>
    {
        public Guid Id { get; set; }
        public EntityName Name { get; set; }
        public string Description { get; set; }
    }
}
