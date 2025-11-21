using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Services;
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

namespace Infrastructure.Services.Authentication
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

        public ClaimsPrincipal? ValidateToken(string token, bool validateExpiration = true)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = validateExpiration,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public Guid? GetUserIdFromToken(string token)
        {
            try
            {
                var principal = ValidateToken(token, validateExpiration: false);
                if (principal == null)
                    return null;

                var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return null;

                return userId;
            }
            catch
            {
                return null;
            }
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                return jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }
    }
    
}
