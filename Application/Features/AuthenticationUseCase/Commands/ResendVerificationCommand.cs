using Domain.Base;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class ResendVerificationCommand : IRequest<MessageDTO>
    {
        public string Email { get; set; }
    }
}
