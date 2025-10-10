using Application.Features.AuthenticationUseCase.DTOs;
using Domain.Base;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class RefreshTokenCommand : IRequest<ResultDTO<TokenResultDTO>>
    {
        public string RefreshToken { get; set; }
    }
}
