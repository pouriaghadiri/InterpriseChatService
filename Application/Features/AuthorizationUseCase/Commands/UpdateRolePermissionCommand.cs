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
        public Guid Id { get; set; } // Id of the RolePermission record to update
        public Guid? RoleId { get; set; } // Optional: New RoleId to assign
        public Guid? DepartmentId { get; set; } // Optional: New DepartmentId to assign
        public Guid? PermissionId { get; set; } // Optional: New PermissionId to assign
    }
}
