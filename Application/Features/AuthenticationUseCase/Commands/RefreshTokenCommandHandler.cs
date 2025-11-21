using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Services;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using MediatR;
using System;
using System.Linq;
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

        public RefreshTokenCommandHandler(
            IUserRepository userRepository, 
            IJwtTokenService jwtTokenService,
            ICacheService cacheService,
            IActiveDepartmentService activeDepartmentService,
            IUserRoleInDepartmentRepository userRoleInDepartmentRepository)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _cacheService = cacheService;
            _activeDepartmentService = activeDepartmentService;
            _userRoleInDepartmentRepository = userRoleInDepartmentRepository;
        }

        public async Task<ResultDTO<TokenResultDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Validate refresh token input
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return ResultDTO<TokenResultDTO>.Failure("Invalid", null, "Refresh token is required");
            }

            // Validate the token structure and signature (but allow expired tokens for refresh)
            var principal = _jwtTokenService.ValidateToken(request.RefreshToken, validateExpiration: false);
            if (principal == null)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "Invalid refresh token");
            }

            // Check if token is blacklisted
            var blacklistKey = $"blacklist:{request.RefreshToken}";
            var isBlacklisted = await _cacheService.ExistsAsync(blacklistKey);
            if (isBlacklisted)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "Refresh token has been revoked");
            }

            // Extract user ID from token
            var userId = _jwtTokenService.GetUserIdFromToken(request.RefreshToken);
            if (!userId.HasValue)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "Invalid refresh token: user ID not found");
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

            // Create token response
            var tokenResponse = new TokenResultDTO
            {
                Token = newToken,
                ExpireTime = expireDate
            };

            // Update cache with new token
            var email = user.Email.Value;
            var timeToExpire = expireDate - DateTime.Now;
            await _cacheService.SetAsync<TokenResultDTO>($"UserEmail:{email}", tokenResponse, timeToExpire);

            // Optionally blacklist the old refresh token (token rotation)
            // This is a security best practice - invalidate the old token after use
            await _cacheService.SetAsync(blacklistKey, new { Blacklisted = true, Timestamp = DateTime.UtcNow }, TimeSpan.FromDays(7));

            return ResultDTO<TokenResultDTO>.Success("OK", tokenResponse, "Token refreshed successfully");
        }
    }
}
