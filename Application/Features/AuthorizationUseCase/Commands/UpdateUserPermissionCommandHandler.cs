using Domain.Base;
using Domain.Base.Interface;
using Domain.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Commands
{
    public class UpdateUserPermissionCommandHandler : IRequestHandler<UpdateUserPermissionCommand, MessageDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheInvalidationService _cacheInvalidationService;

        public UpdateUserPermissionCommandHandler(IUnitOfWork unitOfWork, ICacheInvalidationService cacheInvalidationService)
        {
            _unitOfWork = unitOfWork;
            _cacheInvalidationService = cacheInvalidationService;
        }

        public async Task<MessageDTO> Handle(UpdateUserPermissionCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            await _cacheInvalidationService.InvalidateUserPermissionCacheAsync(request.UserId, request.DepartmentId);
            
            return MessageDTO.Success("Updated", "User permissions updated successfully");
        }
    }

   
}
