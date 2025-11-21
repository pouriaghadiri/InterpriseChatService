using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthenticationUseCase.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user, IEnumerable<string> roles, out DateTime expDate, IEnumerable<string> permissions = null);
        ClaimsPrincipal? ValidateToken(string token, bool validateExpiration = true);
        Guid? GetUserIdFromToken(string token);
        bool IsTokenExpired(string token);
    }

}
