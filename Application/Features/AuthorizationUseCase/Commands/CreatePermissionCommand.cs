using Domain.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Commands
{
    class CreatePermissionCommand:IRequest<MessageDTO>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
