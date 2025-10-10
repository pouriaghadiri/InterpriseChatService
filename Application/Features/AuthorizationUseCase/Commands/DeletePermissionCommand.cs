using Domain.Base;
using MediatR;

namespace Application.Features.AuthorizationUseCase.Commands
{
    public class DeletePermissionCommand : IRequest<MessageDTO>
    {
        public Guid Id { get; set; }

        public DeletePermissionCommand(Guid id)
        {
            Id = id;
        }
    }
}
