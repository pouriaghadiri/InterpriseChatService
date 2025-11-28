using Application.Common;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Repositories;
using Domain.Services;
using MediatR;
using System;
using System.Threading.Tasks;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class BlockAllUserTokensCommandHandler : IRequestHandler<BlockAllUserTokensCommand, MessageDTO>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;

        public BlockAllUserTokensCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            ICacheService cacheService,
            IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _cacheService = cacheService;
            _unitOfWork = unitOfWork;
        }

        public async Task<MessageDTO> Handle(BlockAllUserTokensCommand request, CancellationToken cancellationToken)
        {
            // Get all active tokens for the user
            var activeTokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(request.UserId);
            
            if (activeTokens == null || activeTokens.Count == 0)
            {
                return MessageDTO.Success("Success", "No active tokens found to block");
            }

            int blockedCount = 0;

            // Revoke all active tokens
            foreach (var token in activeTokens)
            {
                if (!token.IsRevoked)
                {
                    token.Revoke();
                    if (!string.IsNullOrWhiteSpace(request.Reason))
                    {
                        token.RevokedBy = request.Reason;
                    }
                    await _refreshTokenRepository.UpdateAsync(token);

                    // Remove from cache
                    var refreshTokenCacheKey = CacheHelper.RefreshTokenKey(token.Token);
                    await _cacheService.RemoveAsync(refreshTokenCacheKey);

                    // Add to blacklist in cache
                    var blacklistKey = CacheHelper.TokenBlacklistKey(token.Token);
                    var blacklistData = new 
                    { 
                        Blacklisted = true, 
                        Timestamp = DateTime.Now,
                        Reason = request.Reason
                    };
                    await _cacheService.SetAsync(blacklistKey, blacklistData, CacheHelper.Expiration.TokenBlacklist);

                    blockedCount++;
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MessageDTO.Success("Success", $"Successfully blocked {blockedCount} token(s)");
        }
    }
}

