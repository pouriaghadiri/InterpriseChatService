using Domain.Base;
using Domain.Common.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserUseCase
{
    public class PingCommand : IRequest<string>
    {

    }
}
