using Domain.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Commands
{
    public class UpdateUserPermissionCommand : IRequest<MessageDTO>
    {
        public Guid UserId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid PermissionId { get; set; }
        // Add other properties as needed
    }
}
