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
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;

        public VerifyEmailCommandHandler(IUserRepository userRepository, ICacheService cacheService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _cacheService = cacheService;
            _unitOfWork = unitOfWork;
        }

        public async Task<MessageDTO> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
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
                return MessageDTO.Failure("Invalid Token", null, "Verification token is required");
            }

            // Validate and retrieve token from cache
            var tokenCacheKey = CacheHelper.EmailVerificationTokenKey(request.Token);
            var tokenCacheModel = await _cacheService.GetAsync<EmailVerificationTokenCacheModel>(tokenCacheKey);

            if (tokenCacheModel == null)
            {
                return MessageDTO.Failure("Invalid Token", null, "Invalid or expired verification token");
            }

            // Check if token is expired
            if (DateTime.Now > tokenCacheModel.ExpiresAt)
            {
                // Remove expired token
                await _cacheService.RemoveAsync(tokenCacheKey);
                return MessageDTO.Failure("Expired Token", null, "Verification token has expired. Please request a new one");
            }

            // Verify email matches token
            if (tokenCacheModel.Email != emailRes.Data.Value)
            {
                return MessageDTO.Failure("Invalid Request", null, "Email does not match the verification token");
            }

            // Get user
            var user = await _userRepository.GetbyIdAsync(tokenCacheModel.UserId);
            if (user == null)
            {
                return MessageDTO.Failure("User Not Found", null, "User associated with this token was not found");
            }

            // Verify email matches user's email
            if (user.Email.Value != emailRes.Data.Value)
            {
                return MessageDTO.Failure("Invalid Request", null, "Email does not match the user's email");
            }

            // Check if email is already verified
            var emailCacheKey = CacheHelper.EmailVerificationTokenByEmailKey(emailRes.Data.Value);
            if (user.IsEmailVerified)
            {
                // Invalidate the token even if already verified
                await _cacheService.RemoveAsync(tokenCacheKey);
                await _cacheService.RemoveAsync(emailCacheKey);
                return MessageDTO.Success("Already Verified", "Email is already verified");
            }

            // Mark email as verified
            var markVerifiedResult = user.MarkEmailAsVerified();
            if (!markVerifiedResult.IsSuccess)
            {
                return markVerifiedResult;
            }

            // Save changes to database
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate the verification token and email cache
            await _cacheService.RemoveAsync(tokenCacheKey);
            await _cacheService.RemoveAsync(emailCacheKey);

            return MessageDTO.Success("Success", "Email has been verified successfully");
        }
    }
}
