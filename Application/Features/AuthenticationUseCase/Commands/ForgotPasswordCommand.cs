using Domain.Base;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class ForgotPasswordCommand : IRequest<MessageDTO>
    {
        public string Email { get; set; }
    }
}
