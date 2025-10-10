using Domain.Base;
using Domain.Common.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.RoleUseCase.Commands
{
    public class AddRoleCommand:IRequest<MessageDTO>
    {
        public string Name{ get; set; }
        public string Description { get; set; }
    }
}
