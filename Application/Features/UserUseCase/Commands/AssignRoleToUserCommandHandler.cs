using Domain.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserUseCase.Commands
{
    class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, MessageDTO>
    {

        public async Task<MessageDTO> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
