using Application.Common;
using Application.Common.CacheModels;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Repositories;
using Domain.Services;
using MediatR;
using System;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public ResetPasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<MessageDTO> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // Validate email
            var emailRes = Email.Create(request.Email);
            if (!emailRes.IsSuccess)
            {
                return MessageDTO.Failure("Invalid Email", emailRes.Errors, "Please provide a valid email address");
            }

            // Validate token is provided
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return MessageDTO.Failure("Invalid Token", null, "Reset token is required");
            }

            // Validate passwords match
            if (request.NewPassword != request.ConfirmPassword)
            {
                return MessageDTO.Failure("Password Mismatch", null, "New password and confirmation do not match");
            }

            // Validate password strength
            var passwordRes = HashedPassword.Create(request.NewPassword);
            if (!passwordRes.IsSuccess)
            {
                return MessageDTO.Failure("Invalid Password", passwordRes.Errors, "Password does not meet requirements");
            }

            // Validate and retrieve token from cache
            var tokenCacheKey = CacheHelper.PasswordResetTokenKey(request.Token);
            var tokenCacheModel = await _cacheService.GetAsync<PasswordResetTokenCacheModel>(tokenCacheKey);

            if (tokenCacheModel == null)
            {
                return MessageDTO.Failure("Invalid Token", null, "Invalid or expired reset token");
            }

            // Check if token is expired
            if (DateTime.UtcNow > tokenCacheModel.ExpiresAt)
            {
                // Remove expired token
                await _cacheService.RemoveAsync(tokenCacheKey);
                return MessageDTO.Failure("Expired Token", null, "Reset token has expired. Please request a new one");
            }

            // Verify email matches token
            if (tokenCacheModel.Email != emailRes.Data.Value)
            {
                return MessageDTO.Failure("Invalid Request", null, "Email does not match the reset token");
            }

            // Get user
            var user = await _userRepository.GetbyIdAsync(tokenCacheModel.UserId);
            if (user == null)
            {
                return MessageDTO.Failure("User Not Found", null, "User associated with this token was not found");
            }

            // Update user's password (for password reset, we don't need current password)
            user.HashedPassword = passwordRes.Data;
            user.Update();

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate the reset token and email cache
            await _cacheService.RemoveAsync(tokenCacheKey);
            var emailCacheKey = CacheHelper.PasswordResetTokenByEmailKey(emailRes.Data.Value);
            await _cacheService.RemoveAsync(emailCacheKey);

            return MessageDTO.Success("Success", "Password has been reset successfully");
        }
    }
}
