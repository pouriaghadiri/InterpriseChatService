using Domain.Base;
using Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using Application.Common;

namespace Application.Features.AuthenticationUseCase.Commands
{
    /// <summary>
    /// Enhanced logout handler with token blacklisting and comprehensive cache invalidation
    /// </summary>
    public class LogoutCommandHandlerEnhanced : IRequestHandler<LogoutCommand, MessageDTO>
    {
        private readonly ICacheInvalidationService _cacheInvalidationService;
        private readonly ICacheService _cacheService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogoutCommandHandlerEnhanced(
            ICacheInvalidationService cacheInvalidationService, 
            ICacheService cacheService,
            IHttpContextAccessor httpContextAccessor)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _cacheService = cacheService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MessageDTO> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get user ID from JWT token claims
                var userId = GetUserIdFromToken();
                
                if (userId == null)
                {
                    return MessageDTO.Failure("Unauthorized", null, "User not found in token");
                }

                // Get the JWT token from Authorization header for blacklisting
                var token = GetJwtTokenFromHeader();
                
                // 1. Invalidate all user caches
                await _cacheInvalidationService.InvalidateUserCacheAsync(userId.Value);

                // 2. Add token to blacklist (if token exists)
                if (!string.IsNullOrEmpty(token))
                {
                    await BlacklistTokenAsync(token);
                }

                // 3. Remove any refresh tokens (if you have refresh token implementation)
                await RemoveRefreshTokensAsync(userId.Value);

                // 4. Log the logout event (you can add logging here)
                // _logger.LogInformation("User {UserId} logged out successfully", userId);

                return MessageDTO.Success("Success", "Logged out successfully");
            }
            catch (Exception ex)
            {
                // Log the exception but don't expose internal details
                // _logger.LogError(ex, "Error occurred during logout for user");
                return MessageDTO.Failure("Error", null, "An error occurred during logout");
            }
        }

        private Guid? GetUserIdFromToken()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.User?.Identity?.IsAuthenticated != true)
                {
                    return null;
                }

                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return null;
                }

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private string GetJwtTokenFromHeader()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var authHeader = httpContext?.Request?.Headers["Authorization"].FirstOrDefault();
                
                if (authHeader != null && authHeader.StartsWith("Bearer "))
                {
                    return authHeader.Substring("Bearer ".Length).Trim();
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        private async Task BlacklistTokenAsync(string token)
        {
            try
            {
                // Add token to blacklist with expiration time
                var blacklistKey = CacheHelper.TokenBlacklistKey(token);
                var blacklistValue = new { Blacklisted = true, Timestamp = DateTime.UtcNow };
                await _cacheService.SetAsync(blacklistKey, blacklistValue, CacheHelper.Expiration.TokenBlacklist);
            }
            catch
            {
                // Log error but don't throw
            }
        }

        private async Task RemoveRefreshTokensAsync(Guid userId)
        {
            try
            {
                // Remove refresh tokens for the user
                var refreshTokenKey = $"refresh_token:{userId}";
                await _cacheService.RemoveAsync(refreshTokenKey);
            }
            catch
            {
                // Log error but don't throw
            }
        }
    }
}
