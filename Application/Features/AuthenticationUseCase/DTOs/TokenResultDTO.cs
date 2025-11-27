using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthenticationUseCase.DTOs
{
    public class TokenResultDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
