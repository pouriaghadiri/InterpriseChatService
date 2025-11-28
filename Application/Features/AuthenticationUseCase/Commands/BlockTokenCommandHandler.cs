using Application.Common;
using Application.Features.AuthenticationUseCase.Services;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Repositories;
using Domain.Services;
using MediatR;
using System;
using System.Threading.Tasks;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class BlockTokenCommandHandler : IRequestHandler<BlockTokenCommand, MessageDTO>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICacheService _cacheService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;

        public BlockTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            ICacheService cacheService,
            IJwtTokenService jwtTokenService,
            IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _cacheService = cacheService;
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<MessageDTO> Handle(BlockTokenCommand request, CancellationToken cancellationToken)
        {
            // Validate token input
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return MessageDTO.Failure("Invalid", null, "Token is required");
            }

            // Validate token structure (works for both access and refresh tokens)
            var principal = _jwtTokenService.ValidateToken(request.Token, validateExpiration: false);
            if (principal == null)
            {
                return MessageDTO.Failure("Invalid", null, "Invalid token format");
            }

            // Check if already blacklisted
            var blacklistKey = CacheHelper.TokenBlacklistKey(request.Token);
            var isAlreadyBlacklisted = await _cacheService.ExistsAsync(blacklistKey);
            if (isAlreadyBlacklisted)
            {
                return MessageDTO.Failure("AlreadyBlocked", null, "Token is already blocked");
            }

            // Try to get refresh token from database (if it's a refresh token)
            var refreshTokenEntity = await _refreshTokenRepository.GetByTokenAsync(request.Token);
            
            if (refreshTokenEntity != null)
            {
                // It's a refresh token - revoke in database
                if (refreshTokenEntity.IsRevoked)
                {
                    return MessageDTO.Failure("AlreadyBlocked", null, "Token is already blocked");
                }

                refreshTokenEntity.Revoke();
                if (!string.IsNullOrWhiteSpace(request.Reason))
                {
                    refreshTokenEntity.RevokedBy = request.Reason;
                }
                await _refreshTokenRepository.UpdateAsync(refreshTokenEntity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Remove refresh token from cache
                var refreshTokenCacheKey = CacheHelper.RefreshTokenKey(request.Token);
                await _cacheService.RemoveAsync(refreshTokenCacheKey);
            }
            // If it's an access token (not in refresh token DB), we'll just blacklist it in cache

            // Add to blacklist in cache (works for both access and refresh tokens)
            var blacklistData = new 
            { 
                Blacklisted = true, 
                Timestamp = DateTime.Now,
                Reason = request.Reason,
                TokenType = refreshTokenEntity != null ? "refresh" : "access"
            };
            await _cacheService.SetAsync(blacklistKey, blacklistData, CacheHelper.Expiration.TokenBlacklist);

            return MessageDTO.Success("Success", "Token has been blocked successfully");
        }
    }
}

