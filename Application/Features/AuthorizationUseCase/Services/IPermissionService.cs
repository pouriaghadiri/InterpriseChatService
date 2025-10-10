using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Services
{
    public interface IPermissionService
    {
        Task<bool> UserHasPermissionAsync(Guid userId, Guid departmentId, string permissionKey);


    }
}
