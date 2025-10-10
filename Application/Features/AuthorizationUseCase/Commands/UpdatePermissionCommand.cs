using Domain.Base;
using MediatR;

namespace Application.Features.AuthorizationUseCase.Commands
{
    public class UpdatePermissionCommand : IRequest<MessageDTO>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
