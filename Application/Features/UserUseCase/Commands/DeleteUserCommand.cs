using Domain.Base;
using MediatR;

namespace Application.Features.UserUseCase.Commands
{
    public class DeleteUserCommand : IRequest<MessageDTO>
    {
        public Guid Id { get; set; }
    }
}

