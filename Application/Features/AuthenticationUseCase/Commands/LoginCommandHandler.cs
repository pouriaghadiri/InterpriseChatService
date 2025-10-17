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

        public LoginCommandHandler(IUserRepository userRepository, IJwtTokenService jwtTokenService,
                                   ICacheService cacheService, IActiveDepartmentService activeDepartmentService)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _cacheService = cacheService;
            _activeDepartmentService = activeDepartmentService;
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

            var roles = userRes.Result.UserRoles.Select(x => x.Role.Name.Value).ToList();

            //// اگر Permission داریم از Role->Permission map یا جدولی بگیر:
            //var permissions = new List<string>();
            //foreach (var ur in user.UserRoles)
            //{
            //    // اگر نقش شامل Permission collection است:
            //    if (ur.Role.Permissions != null)
            //        permissions.AddRange(ur.Role.Permissions.Select(p => p.Name));
            //}

            var token = _jwtTokenService.GenerateToken(userRes.Result, roles, out DateTime expireDate);

            // Save user's active department to Redis
            var user = userRes.Result;
            if (user.ActiveDepartmentId.HasValue)
            {
                // If user already has an active department, cache it
                await _activeDepartmentService.SetActiveDepartmentIdAsync(user.Id, user.ActiveDepartmentId.Value);
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
                await _activeDepartmentService.SetActiveDepartmentIdAsync(user.Id, firstDepartment.Id);
                
            }

            var tokenResponse = new TokenResultDTO
            {
                Token = token,
                ExpireTime = expireDate
            };

            await _cacheService.SetAsync<TokenResultDTO>($"UserEmail:{request.Email}", tokenResponse, expireDate.TimeOfDay - DateTime.Now.TimeOfDay);
            return ResultDTO<TokenResultDTO>.Success("OK", tokenResponse, "Logged in");


        }

    }
}
