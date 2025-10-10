using Domain.Base;
using MediatR;
using System;

namespace Application.Features.DepartmentUseCase.Commands
{
    public class UpdateDepartmentCommand : IRequest<MessageDTO>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
