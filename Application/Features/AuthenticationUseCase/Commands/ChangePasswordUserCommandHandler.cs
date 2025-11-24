using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class ChangePasswordUserCommandHandler : IRequestHandler<ChangePasswordUserCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheInvalidationService _cacheInvalidationService;

        public ChangePasswordUserCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, ICacheInvalidationService cacheInvalidationService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _cacheInvalidationService = cacheInvalidationService;
        }

        public async Task<MessageDTO> Handle(ChangePasswordUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetbyIdAsync(request.Id);
            if (user == null)
                return MessageDTO.Failure("Not Found Error", null, "This user is not valid!");
            
            if(!user.HashedPassword.Verify(request.CurrentPassword))
                return MessageDTO.Failure("Error", null, "The current password is incorrect.");
            
            if(request.NewPassword != request.NewPasswordConfirm)
                return MessageDTO.Failure("Error", null, "The new password and confirmation password do not match.");

            user.HashedPassword = HashedPassword.CreateFromPlain(request.NewPassword);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate user cache (password change might affect token validation)
            await _cacheInvalidationService.InvalidateUserCacheAsync(user.Id, user.Email.Value);

            return MessageDTO.Success("Updated", "Password changed successfully.");
        }
    }
}
