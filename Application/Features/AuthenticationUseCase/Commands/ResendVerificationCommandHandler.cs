using Application.Common;
using Application.Common.CacheModels;
using Application.Common.DTOs;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Repositories;
using Domain.Services;
using MediatR;
using System;
using System.Net;
using System.Security.Cryptography;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class ResendVerificationCommandHandler : IRequestHandler<ResendVerificationCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        private readonly IEmailService _emailService;
        private readonly EmailSettingsDTO _emailSettings;

        public ResendVerificationCommandHandler(
            IUserRepository userRepository,
            ICacheService cacheService,
            IEmailService emailService,
            EmailSettingsDTO emailSettings)
        {
            _userRepository = userRepository;
            _cacheService = cacheService;
            _emailService = emailService;
            _emailSettings = emailSettings;
        }

        public async Task<MessageDTO> Handle(ResendVerificationCommand request, CancellationToken cancellationToken)
        {
            var emailRes = Email.Create(request.Email);
            if (!emailRes.IsSuccess)
            {
                return MessageDTO.Failure("Invalid Email", emailRes.Errors, "Please provide a valid email address");
            }

            var user = await _userRepository.GetbyEmailAsync(emailRes.Data);
            if (user == null)
            {
                // For security, don't reveal if email exists or not
                return MessageDTO.Success("Success", "If the email exists, a verification email has been sent");
            }

            // Check if email is already verified
            if (user.IsEmailVerified)
            {
                return MessageDTO.Success("Already Verified", "Email is already verified. No need to resend verification email");
            }

            // Check if there's an existing verification token for this email and invalidate it
            var emailCacheKey = CacheHelper.EmailVerificationTokenByEmailKey(emailRes.Data.Value);
            var existingToken = await _cacheService.GetAsync<string>(emailCacheKey);
            if (existingToken != null)
            {
                // Invalidate the previous token
                var previousTokenKey = CacheHelper.EmailVerificationTokenKey(existingToken);
                await _cacheService.RemoveAsync(previousTokenKey);
            }

            // Generate a secure email verification token
            var verificationToken = GenerateSecureToken();
            var expiresAt = DateTime.UtcNow.Add(CacheHelper.Expiration.EmailVerificationToken);

            // Store token in cache with expiration
            var tokenCacheModel = new EmailVerificationTokenCacheModel
            {
                UserId = user.Id,
                Email = emailRes.Data.Value,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };

            var tokenCacheKey = CacheHelper.EmailVerificationTokenKey(verificationToken);

            // Store token by token value
            await _cacheService.SetAsync(tokenCacheKey, tokenCacheModel, CacheHelper.Expiration.EmailVerificationToken);

            // Store token by email (to track and invalidate previous tokens for the same email)
            await _cacheService.SetAsync(emailCacheKey, verificationToken, CacheHelper.Expiration.EmailVerificationToken);

            // Generate verification link
            var verificationLink = $"{_emailSettings.BaseUrl}/verify-email?token={WebUtility.UrlEncode(verificationToken)}&email={WebUtility.UrlEncode(emailRes.Data.Value)}";

            // Send verification email
            var emailSent = await _emailService.SendEmailVerificationEmailAsync(user.Email.Value, verificationToken, verificationLink);

            if (!emailSent)
            {
                // Log the error but don't reveal it to the user for security
                // In production, you might want to log this to a monitoring system
                return MessageDTO.Success("Success", "If the email exists, a verification email has been sent");
            }

            return MessageDTO.Success("Success", "If the email exists, a verification email has been sent");
        }

        private string GenerateSecureToken()
        {
            // Generate a cryptographically secure random token
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[32]; // 256 bits
                rng.GetBytes(bytes);
                var base64Token = Convert.ToBase64String(bytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", ""); // URL-safe base64

                // Return a 32-character token (or full length if shorter)
                return base64Token.Length > 32 ? base64Token.Substring(0, 32) : base64Token;
            }
        }
    }
}
