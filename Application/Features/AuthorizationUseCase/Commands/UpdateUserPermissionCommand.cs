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
        public Guid Id { get; set; } // Id of the UserPermission record to update
        public Guid? UserId { get; set; } // Optional: New UserId to assign
        public Guid? DepartmentId { get; set; } // Optional: New DepartmentId to assign
        public Guid? PermissionId { get; set; } // Optional: New PermissionId to assign
    }
}
