using Application.Common;
using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Services;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ResultDTO<TokenResultDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ICacheService _cacheService;
        private readonly IActiveDepartmentService _activeDepartmentService;
        private readonly IUserRoleInDepartmentRepository _userRoleInDepartmentRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RefreshTokenCommandHandler(
            IUserRepository userRepository, 
            IJwtTokenService jwtTokenService,
            ICacheService cacheService,
            IActiveDepartmentService activeDepartmentService,
            IUserRoleInDepartmentRepository userRoleInDepartmentRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _cacheService = cacheService;
            _activeDepartmentService = activeDepartmentService;
            _userRoleInDepartmentRepository = userRoleInDepartmentRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultDTO<TokenResultDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Validate refresh token input
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return ResultDTO<TokenResultDTO>.Failure("Invalid", null, "Refresh token is required");
            }

            // Check if token is blacklisted in cache first (early exit for security)
            var blacklistKey = CacheHelper.TokenBlacklistKey(request.RefreshToken);
            var isBlacklisted = await _cacheService.ExistsAsync(blacklistKey);
            if (isBlacklisted)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "Refresh token has been revoked");
            }

            // Hybrid approach: Check cache first (fast path)
            var refreshTokenCacheKey = CacheHelper.RefreshTokenKey(request.RefreshToken);
            var cachedToken = await _cacheService.GetAsync<object>(refreshTokenCacheKey);

            RefreshToken? refreshTokenEntity = null;
            
            if (cachedToken != null)
            {
                // Token found in cache, now validate in database
                refreshTokenEntity = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            }
            else
            {
                // Not in cache, check database (slower but persistent)
                refreshTokenEntity = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            }

            // Validate token exists and is active
            if (refreshTokenEntity == null)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "Refresh token not found");
            }

            if (refreshTokenEntity.IsRevoked)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "Refresh token has been revoked");
            }

            if (refreshTokenEntity.IsExpired)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "Refresh token has expired");
            }

            // Validate the token structure and signature (but allow expired tokens for refresh)
            var principal = _jwtTokenService.ValidateToken(request.RefreshToken, validateExpiration: false);
            if (principal == null)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "Invalid refresh token signature");
            }

            // Extract user ID from token using the service method (handles all claim type variations)
            var userId = _jwtTokenService.GetUserIdFromToken(request.RefreshToken);
            if (!userId.HasValue)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "Invalid refresh token: unable to extract user ID");
            }

            // Validate that user ID from token matches the database entity
            if (userId.Value != refreshTokenEntity.UserId)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "Invalid refresh token: user ID mismatch");
            }

            // Get user from repository
            var user = await _userRepository.GetbyIdAsync(userId.Value);
            if (user == null)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "User not found");
            }

            // Get user's active department
            Guid activeDepartmentId;
            var cachedDepartmentId = await _activeDepartmentService.GetActiveDepartmentIdAsync(user.Id);
            
            if (cachedDepartmentId.HasValue)
            {
                activeDepartmentId = cachedDepartmentId.Value;
            }
            else if (user.ActiveDepartmentId.HasValue)
            {
                activeDepartmentId = user.ActiveDepartmentId.Value;
                await _activeDepartmentService.SetActiveDepartmentIdAsync(user.Id, activeDepartmentId);
            }
            else
            {
                // If no active department, get the first department from user roles
                var firstDepartment = user.UserRoles
                    .SelectMany(ur => ur.UserRoleInDepartments)
                    .Select(urid => urid.Department)
                    .FirstOrDefault();

                if (firstDepartment == null)
                {
                    return ResultDTO<TokenResultDTO>.Failure("Auth", null, "User has no valid department");
                }

                activeDepartmentId = firstDepartment.Id;
                await _activeDepartmentService.SetActiveDepartmentIdAsync(user.Id, activeDepartmentId);
            }

            // Get roles of user based on the active department
            var rolesInDepartment = await _userRoleInDepartmentRepository.GetRolesOfUserInDepartment(user.Id, activeDepartmentId);
            var roles = rolesInDepartment.Select(x => x.Name.Value).ToList();

            if (roles == null || roles.Count == 0)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "User has no roles in the active department");
            }

            // Generate new access token
            var newToken = _jwtTokenService.GenerateToken(user, roles, out DateTime expireDate);

            // Generate new refresh token (token rotation)
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken(user, out DateTime refreshTokenExpireDate);

            // Revoke old refresh token in database
            refreshTokenEntity.Revoke(newRefreshToken);
            await _refreshTokenRepository.UpdateAsync(refreshTokenEntity);

            // Get device info and IP address from HttpContext
            var deviceInfo = _httpContextAccessor.HttpContext?.GetDeviceInfo();
            var ipAddress = _httpContextAccessor.HttpContext?.GetClientIpAddress();

            // Store new refresh token in database
            var newRefreshTokenEntity = RefreshToken.Create(user.Id, newRefreshToken, refreshTokenExpireDate, deviceInfo, ipAddress);
            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Create token response
            var tokenResponse = new TokenResultDTO
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpireTime = expireDate
            };

            var email = user.Email.Value;

            // Update access token in cache (short-lived)
            var accessTokenCacheKey = CacheHelper.AccessTokenKey(email);
            await _cacheService.SetAsync<TokenResultDTO>(accessTokenCacheKey, tokenResponse, expireDate - DateTime.Now);

            // Remove old refresh token from cache
            await _cacheService.RemoveAsync(refreshTokenCacheKey);

            // Store new refresh token in cache (long-lived)
            var newRefreshTokenCacheKey = CacheHelper.RefreshTokenKey(newRefreshToken);
            var newRefreshTokenCacheData = new { 
                UserId = user.Id, 
                Email = email,
                CreatedAt = DateTime.Now,
                ExpiresAt = refreshTokenExpireDate
            };
            await _cacheService.SetAsync(newRefreshTokenCacheKey, newRefreshTokenCacheData, refreshTokenExpireDate - DateTime.Now);

            // Blacklist old refresh token in cache (additional security layer)
            await _cacheService.SetAsync(blacklistKey, new { Blacklisted = true, Timestamp = DateTime.Now }, CacheHelper.Expiration.TokenBlacklist);

            return ResultDTO<TokenResultDTO>.Success("OK", tokenResponse, "Token refreshed successfully");
        }
    }
}