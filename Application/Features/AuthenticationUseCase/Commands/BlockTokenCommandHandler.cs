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
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return MessageDTO.Failure("Invalid", null, "Refresh token is required");
            }

            // Validate token structure
            var principal = _jwtTokenService.ValidateToken(request.RefreshToken, validateExpiration: false);
            if (principal == null)
            {
                return MessageDTO.Failure("Invalid", null, "Invalid token format");
            }

            // Get token from database
            var refreshTokenEntity = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (refreshTokenEntity == null)
            {
                return MessageDTO.Failure("NotFound", null, "Token not found");
            }

            // Check if already revoked
            if (refreshTokenEntity.IsRevoked)
            {
                return MessageDTO.Failure("AlreadyBlocked", null, "Token is already blocked");
            }

            // Revoke token in database
            refreshTokenEntity.Revoke();
            if (!string.IsNullOrWhiteSpace(request.Reason))
            {
                refreshTokenEntity.RevokedBy = request.Reason;
            }
            await _refreshTokenRepository.UpdateAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Remove from cache
            var refreshTokenCacheKey = CacheHelper.RefreshTokenKey(request.RefreshToken);
            await _cacheService.RemoveAsync(refreshTokenCacheKey);

            // Add to blacklist in cache
            var blacklistKey = CacheHelper.TokenBlacklistKey(request.RefreshToken);
            var blacklistData = new 
            { 
                Blacklisted = true, 
                Timestamp = DateTime.Now,
                Reason = request.Reason
            };
            await _cacheService.SetAsync(blacklistKey, blacklistData, CacheHelper.Expiration.TokenBlacklist);

            return MessageDTO.Success("Success", "Token has been blocked successfully");
        }
    }
}

