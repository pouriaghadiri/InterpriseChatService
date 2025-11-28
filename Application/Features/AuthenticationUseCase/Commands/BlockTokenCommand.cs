using Domain.Base;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class BlockTokenCommand : IRequest<MessageDTO>
    {
        public string RefreshToken { get; set; }
        public string? Reason { get; set; }
    }
}

