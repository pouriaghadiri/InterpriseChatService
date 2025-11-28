using Domain.Base;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    /// <summary>
    /// Command to block/revoke a token (works for both access and refresh tokens)
    /// </summary>
    public class BlockTokenCommand : IRequest<MessageDTO>
    {
        /// <summary>
        /// The token to block (can be access token or refresh token)
        /// </summary>
        public string Token { get; set; }
        
        /// <summary>
        /// Optional reason for blocking the token
        /// </summary>
        public string? Reason { get; set; }
    }
}

