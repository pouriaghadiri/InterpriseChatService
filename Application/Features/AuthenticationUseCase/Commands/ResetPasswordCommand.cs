using Domain.Base;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class ResetPasswordCommand : IRequest<MessageDTO>
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
