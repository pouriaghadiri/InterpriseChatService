using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Services;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class LoginCommandHandler: IRequestHandler<LoginCommand, ResultDTO<TokenResultDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ICacheService _cacheService;
        private readonly IActiveDepartmentService _activeDepartmentService;
        private readonly IUserRoleInDepartmentRepository _userRoleInDepartmentRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(IUserRepository userRepository, IJwtTokenService jwtTokenService,
                                   ICacheService cacheService, IActiveDepartmentService activeDepartmentService,
                                   IUserRoleInDepartmentRepository userRoleInDepartmentRepository,
                                   IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _cacheService = cacheService;
            _activeDepartmentService = activeDepartmentService;
            _userRoleInDepartmentRepository = userRoleInDepartmentRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResultDTO<TokenResultDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var emailRes = Email.Create(request.Email);
            if (!emailRes.IsSuccess)
            {
                return ResultDTO<TokenResultDTO>.Failure("Invalid", null, "Invalid email");
            }

            var userRes = _userRepository.GetbyEmailAsync(emailRes.Data!);
            if (userRes == null || userRes.Result == null)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "Invalid credentials");
            }

            var passwordRes = userRes.Result.HashedPassword.Verify(request.Password);
            if (!passwordRes)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "Invalid credentials");
            }

            var user = userRes.Result;
            Guid activeDepartmentId;

            // Get user's default/active department first
            if (user.ActiveDepartmentId.HasValue)
            {
                // If user already has an active department, use it
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
                
                // Set the first department as active and cache it
                activeDepartmentId = firstDepartment.Id;
                await _activeDepartmentService.SetActiveDepartmentIdAsync(user.Id, activeDepartmentId);
            }

            // Get roles of user based on the default department
            var rolesInDepartment = await _userRoleInDepartmentRepository.GetRolesOfUserInDepartment(user.Id, activeDepartmentId);
            var roles = rolesInDepartment.Select(x => x.Name.Value).ToList();

            if (roles == null || roles.Count == 0)
            {
                return ResultDTO<TokenResultDTO>.Failure("Auth", null, "User has no roles in the active department");
            }

            //// اگر Permission داریم از Role->Permission map یا جدولی بگیر:
            //var permissions = new List<string>();
            //foreach (var ur in user.UserRoles)
            //{
            //    // اگر نقش شامل Permission collection است:
            //    if (ur.Role.Permissions != null)
            //        permissions.AddRange(ur.Role.Permissions.Select(p => p.Name));
            //}

            // Generate token with roles based on the default department
            var token = _jwtTokenService.GenerateToken(user, roles, out DateTime expireDate);

            // Generate refresh token
            var refreshToken = _jwtTokenService.GenerateRefreshToken(user, out DateTime refreshTokenExpireDate);

            // Store refresh token in database (hybrid approach)
            var refreshTokenEntity = RefreshToken.Create(user.Id, refreshToken, refreshTokenExpireDate);
            await _refreshTokenRepository.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var tokenResponse = new TokenResultDTO
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpireTime = expireDate
            };

            // Store access token in cache (short-lived, for quick lookup)
            var accessTokenCacheKey = $"AccessToken:{request.Email}";
            await _cacheService.SetAsync<TokenResultDTO>(accessTokenCacheKey, tokenResponse, expireDate - DateTime.Now);

            // Store refresh token in cache (long-lived, for quick validation)
            var refreshTokenCacheKey = $"RefreshToken:{refreshToken}";
            var refreshTokenCacheData = new { 
                UserId = user.Id, 
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = refreshTokenExpireDate
            };
            await _cacheService.SetAsync(refreshTokenCacheKey, refreshTokenCacheData, refreshTokenExpireDate - DateTime.Now);

            return ResultDTO<TokenResultDTO>.Success("OK", tokenResponse, "Logged in");


        }

    }
}
