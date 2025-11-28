using Application.Common;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Repositories;
using Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, MessageDTO>
    {
        private readonly ICacheInvalidationService _cacheInvalidationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(
            ICacheInvalidationService cacheInvalidationService, 
            IHttpContextAccessor httpContextAccessor,
            IRefreshTokenRepository refreshTokenRepository,
            ICacheService cacheService,
            IUnitOfWork unitOfWork)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _httpContextAccessor = httpContextAccessor;
            _refreshTokenRepository = refreshTokenRepository;
            _cacheService = cacheService;
            _unitOfWork = unitOfWork;
        }

        public async Task<MessageDTO> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get user ID from JWT token claims
                var userId = _httpContextAccessor.HttpContext?.User?.GetUserId();
                
                if (userId == null)
                {
                    return MessageDTO.Failure("Unauthorized", null, "User not found in token");
                }

                // Revoke all active refresh tokens for the user (hybrid approach)
                var activeTokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(userId.Value);
                foreach (var token in activeTokens)
                {
                    token.Revoke();
                    await _refreshTokenRepository.UpdateAsync(token);
                    
                    // Remove from cache
                    var cacheKey = CacheHelper.RefreshTokenKey(token.Token);
                    await _cacheService.RemoveAsync(cacheKey);
                    
                    // Add to blacklist
                    var blacklistKey = CacheHelper.TokenBlacklistKey(token.Token);
                    await _cacheService.SetAsync(blacklistKey, new { Blacklisted = true, Timestamp = DateTime.Now }, CacheHelper.Expiration.TokenBlacklist);
                }
                
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Invalidate all user caches
                await _cacheInvalidationService.InvalidateUserCacheAsync(userId.Value);

                return MessageDTO.Success("Success", "Logged out successfully");
            }
            catch (Exception ex)
            {
                // Log the exception but don't expose internal details
                return MessageDTO.Failure("Error", null, "An error occurred during logout");
            }
        }
    }
}
