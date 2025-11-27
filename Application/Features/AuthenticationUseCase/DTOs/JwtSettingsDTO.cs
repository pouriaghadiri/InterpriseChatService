using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthenticationUseCase.DTOs
{
    public class JwtSettingsDTO
    {
        public string Key { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int ExpireMinutes { get; set; }
        public int RefreshTokenExpireDays { get; set; } = 7;
    }
}
