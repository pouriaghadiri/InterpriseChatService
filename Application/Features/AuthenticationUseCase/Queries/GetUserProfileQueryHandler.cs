using Application.Common;
using Application.Features.AuthenticationUseCase.DTOs;
using Domain.Base;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.AuthenticationUseCase.Queries
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, ResultDTO<UserProfileDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUserProfileQueryHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultDTO<UserProfileDTO>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            // Get current user ID from JWT token
            var userId = _httpContextAccessor.HttpContext?.User?.GetUserId();
            if (userId == null)
            {
                return ResultDTO<UserProfileDTO>.Failure("Unauthorized", null, "Unable to identify user from token.");
            }

            var user = await _userRepository.GetbyIdAsync(userId.Value);
            if (user == null)
            {
                return ResultDTO<UserProfileDTO>.Failure("NotFound Error", null, "User not found!");
            }

            var profileDTO = new UserProfileDTO
            {
                Id = user.Id,
                FirstName = user.FullName.FirstName,
                LastName = user.FullName.LastName,
                Email = user.Email.Value,
                Phone = user.Phone?.Value,
                Bio = user.Bio,
                Location = user.Location,
                ProfilePicture = user.ProfilePicture,
                IsEmailVerified = user.IsEmailVerified,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return ResultDTO<UserProfileDTO>.Success("Retrieved", profileDTO, "User profile retrieved successfully.");
        }
    }
}
