using Domain.Base;
using Domain.Base.Interface;
using Domain.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Commands
{
    public class UpdateRolePermissionCommandHandler : IRequestHandler<UpdateRolePermissionCommand, MessageDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheInvalidationService _cacheInvalidationService;

        public UpdateRolePermissionCommandHandler(IUnitOfWork unitOfWork, ICacheInvalidationService cacheInvalidationService)
        {
            _unitOfWork = unitOfWork;
            _cacheInvalidationService = cacheInvalidationService;
        }

        public async Task<MessageDTO> Handle(UpdateRolePermissionCommand request, CancellationToken cancellationToken)
        {

            throw new NotImplementedException();
            await _cacheInvalidationService.InvalidateRolePermissionCacheAsync(request.RoleId, request.DepartmentId);
            
            return MessageDTO.Success("Updated", "Role permissions updated successfully");
        }
    }

    
}
