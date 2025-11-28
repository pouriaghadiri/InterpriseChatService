using Domain.Base;
using MediatR;
using System;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class BlockAllUserTokensCommand : IRequest<MessageDTO>
    {
        public Guid UserId { get; set; }
        public string? Reason { get; set; }
    }
}

