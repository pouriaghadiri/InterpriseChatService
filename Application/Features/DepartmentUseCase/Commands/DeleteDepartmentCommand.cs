using Domain.Base;
using MediatR;
using System;

namespace Application.Features.DepartmentUseCase.Commands
{
    public class DeleteDepartmentCommand : IRequest<MessageDTO>
    {
        public Guid Id { get; set; }

        public DeleteDepartmentCommand(Guid id)
        {
            Id = id;
        }
    }
}
