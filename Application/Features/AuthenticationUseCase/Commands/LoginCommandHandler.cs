using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Interfaces;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
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

        public LoginCommandHandler(IUserRepository userRepository, IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;            
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

            var tokenResponse = new TokenResultDTO
            {
                Token = token,
                ExpireTime = expireDate
            };

            return ResultDTO<TokenResultDTO>.Success("OK", tokenResponse, "Logged in");


        }

    }
}
