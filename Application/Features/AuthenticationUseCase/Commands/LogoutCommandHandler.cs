using Domain.Base;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, MessageDTO>
    {
        public LogoutCommandHandler()
        {
        }

        public async Task<MessageDTO> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            // In a real implementation, you would:
            // 1. Add the token to a blacklist
            // 2. Remove refresh tokens
            // 3. Log the logout event
            
            return MessageDTO.Success("Success", "Logged out successfully");
        }
    }
}
