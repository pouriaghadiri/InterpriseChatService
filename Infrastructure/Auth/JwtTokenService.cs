using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Interfaces;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Auth
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettingsDTO _jwtSettings;
        public JwtTokenService(JwtSettingsDTO jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }
        public string GenerateToken(User user, IEnumerable<string> roles, out DateTime expireDate, IEnumerable<string> permissions = null)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FullName.FirstName} {user.FullName.LastName}"),
                new Claim(ClaimTypes.Email, user.Email.Value)
            };

            // نقش‌ها (می‌تواند چندتایی باشد)
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            //// پرمیشن‌ها به صورت claim جدا (برای policy-based)
            //claims.AddRange(permissions.Select(p => new Claim("permission", p)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            expireDate = DateTime.Now.AddMinutes(_jwtSettings.ExpireMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expireDate,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    
}
