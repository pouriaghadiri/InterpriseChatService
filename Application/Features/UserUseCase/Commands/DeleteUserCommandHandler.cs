using Domain.Base;
using Domain.Base.Interface;
using Domain.Repositories;
using Domain.Services;
using MediatR;

namespace Application.Features.UserUseCase.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheInvalidationService _cacheInvalidationService;

        public DeleteUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ICacheInvalidationService cacheInvalidationService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _cacheInvalidationService = cacheInvalidationService;
        }

        public async Task<MessageDTO> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                return MessageDTO.Failure("Invalid Input", null, "User ID is required.");
            }

            var user = await _userRepository.GetbyIdAsync(request.Id);
            if (user == null)
            {
                return MessageDTO.Failure("NotFound Error", null, "User is not found!");
            }

            // Check if user has any active roles or permissions (optional validation)
            // You might want to add business logic here to prevent deletion of users with active assignments

            await _userRepository.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate user cache
            await _cacheInvalidationService.InvalidateUserCacheAsync(user.Id, user.Email.Value);

            return MessageDTO.Success("Deleted", "User deleted successfully.");
        }
    }
}

