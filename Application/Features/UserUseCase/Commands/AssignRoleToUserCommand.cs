using Domain.Base;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserUseCase.Commands
{
    class AssignRoleToUserCommand:IRequest<MessageDTO>
    {
        public Role Role{ get; set; }
        public Department Department { get; set; }
        public User User { get; set; }
    }
}
