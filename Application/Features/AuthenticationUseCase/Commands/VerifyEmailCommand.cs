using Domain.Base;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class VerifyEmailCommand : IRequest<MessageDTO>
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
