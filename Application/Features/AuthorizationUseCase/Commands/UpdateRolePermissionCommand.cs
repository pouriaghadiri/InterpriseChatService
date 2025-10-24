using Domain.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Commands
{
    public class UpdateRolePermissionCommand : IRequest<MessageDTO>
    {
        public Guid RoleId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid PermissionId { get; set; }
        // Add other properties as needed
    }
}
