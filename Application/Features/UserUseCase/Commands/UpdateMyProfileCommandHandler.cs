using Application.Common;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Repositories;
using Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.UserUseCase.Commands
{
    public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheInvalidationService _cacheInvalidationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateMyProfileCommandHandler(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            ICacheInvalidationService cacheInvalidationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _cacheInvalidationService = cacheInvalidationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MessageDTO> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
        {
            // Get current user ID from JWT token
            var userId = _httpContextAccessor.HttpContext?.User?.GetUserId();
            if (userId == null)
            {
                return MessageDTO.Failure("Unauthorized", null, "Unable to identify user from token.");
            }

            var user = await _userRepository.GetbyIdAsync(userId.Value);
            if (user == null)
                return MessageDTO.Failure("NotFound Error", null, "This user is not valid!");

            var createdFullName = PersonFullName.Create(request.FirstName, request.LastName);
            if (!createdFullName.IsSuccess || createdFullName.Data == null)
            {
                return MessageDTO.Failure("Invalid input", null, "Fullname validation failed");
            }
            PersonFullName personFullName = createdFullName.Data;

            var phoneResult = PhoneNumber.Create(request.PhoneNumber);
            if (!phoneResult.IsSuccess)
                return MessageDTO.Failure("Invalid input", phoneResult.Errors, "Phone number validation failed!");

            var result = user.UpdateUserProfile(personFullName, user.Email, phoneResult.Data, request.Bio, request.Location, request.ProfilePicture);
            if (!result.IsSuccess)
            {
                return result;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate user cache (by ID and email)
            await _cacheInvalidationService.InvalidateUserCacheAsync(user.Id, user.Email.Value);

            return MessageDTO.Success("Updated", "User profile Updated successfully.");
        }
    }
}

