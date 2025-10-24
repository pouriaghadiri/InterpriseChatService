using Domain.Base;
using Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, MessageDTO>
    {
        private readonly ICacheInvalidationService _cacheInvalidationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogoutCommandHandler(ICacheInvalidationService cacheInvalidationService, IHttpContextAccessor httpContextAccessor)
        {
            _cacheInvalidationService = cacheInvalidationService;
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
    }
}
