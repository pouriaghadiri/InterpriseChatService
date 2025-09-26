using Application.Features.AuthenticationUseCase.DTOs;
using Domain.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class LoginCommand: IRequest<ResultDTO<TokenResultDTO>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
