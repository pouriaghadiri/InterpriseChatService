using Domain.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Commands
{
    class AssignPermissionToUserCommand : IRequest<MessageDTO>
    {
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
        public Guid DepartmentId { get; set; }
    }
}
