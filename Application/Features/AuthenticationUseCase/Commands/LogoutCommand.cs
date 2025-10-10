using Domain.Base;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class LogoutCommand : IRequest<MessageDTO>
    {
        // This command can be extended to include token blacklisting logic
    }
}
