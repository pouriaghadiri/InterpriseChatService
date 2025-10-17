using Application.Common;
using Application.Features.UserUseCase.DTOs;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Repositories;
using Domain.Services;
using MediatR;

namespace Application.Features.UserUseCase.Queries
{
    public class GetUserByEmailQueryHandlerWithCache : IRequestHandler<GetUserByEmailQuery, ResultDTO<UserDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;

        public GetUserByEmailQueryHandlerWithCache(IUserRepository userRepository, ICacheService cacheService)
        {
            _userRepository = userRepository;
            _cacheService = cacheService;
        }

        public async Task<ResultDTO<UserDTO>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var createEmail = Email.Create(request.Email);
            if (!createEmail.IsSuccess || createEmail.Data == null)
            {
                return ResultDTO<UserDTO>.Failure("Invalid input", null, "Email validation failed");
            }

            // Try to get from cache first
            var cacheKey = CacheHelper.UserByEmailKey(request.Email);
            var cachedUser = await _cacheService.GetAsync<UserDTO>(cacheKey);
            
            if (cachedUser != null)
            {
                return ResultDTO<UserDTO>.Success("From Cache", cachedUser, "User retrieved from cache successfully");
            }

            // Get from database
            var user = await _userRepository.GetbyEmailAsync(createEmail.Data);
            if (user == null)
            {
                return ResultDTO<UserDTO>.Failure("NotFound Error", null, "User is not found!");
            }

            var userResult = new UserDTO
            {
                ID = user.Id,
                FullName = user.FullName,
                UserRoleInDepartments = user.UserRoles.SelectMany(s => s.UserRoleInDepartments.Select(r => new UserRoleInDepartmentDTO
                {
                    DepartmentID = r.DepartmentId,
                    DepartmentName = r.Department.Name,
                    RoleID = r.UserRole.RoleId,
                    RoleName = r.UserRole.Role.Name,
                })).ToList(),
            };

            // Cache the result
            await _cacheService.SetAsync(cacheKey, userResult, CacheHelper.Expiration.User);

            return ResultDTO<UserDTO>.Success("TransferData", userResult, "Transfer data completed successfully");
        }
    }
}
