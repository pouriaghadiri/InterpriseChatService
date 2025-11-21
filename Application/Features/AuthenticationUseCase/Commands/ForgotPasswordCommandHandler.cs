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
using System.Text;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        private readonly IEmailService _emailService;
        private readonly EmailSettingsDTO _emailSettings;

        public ForgotPasswordCommandHandler(
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

        public async Task<MessageDTO> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
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
                return MessageDTO.Success("Success", "If the email exists, a password reset link has been sent");
            }

            // Check if there's an existing token for this email and invalidate it
            var emailCacheKey = CacheHelper.PasswordResetTokenByEmailKey(emailRes.Data.Value);
            var existingToken = await _cacheService.GetAsync<string>(emailCacheKey);
            if (existingToken != null)
            {
                // Invalidate the previous token
                var previousTokenKey = CacheHelper.PasswordResetTokenKey(existingToken);
                await _cacheService.RemoveAsync(previousTokenKey);
            }

            // Generate a secure password reset token
            var resetToken = GenerateSecureToken();
            var expiresAt = DateTime.UtcNow.Add(CacheHelper.Expiration.PasswordResetToken);

            // Store token in cache with expiration
            var tokenCacheModel = new PasswordResetTokenCacheModel
            {
                UserId = user.Id,
                Email = emailRes.Data.Value,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };

            var tokenCacheKey = CacheHelper.PasswordResetTokenKey(resetToken);

            // Store token by token value
            await _cacheService.SetAsync(tokenCacheKey, tokenCacheModel, CacheHelper.Expiration.PasswordResetToken);

            // Store token by email (to track and invalidate previous tokens for the same email)
            await _cacheService.SetAsync(emailCacheKey, resetToken, CacheHelper.Expiration.PasswordResetToken);

            // Generate reset link
            var resetLink = $"{_emailSettings.BaseUrl}/reset-password?token={WebUtility.UrlEncode(resetToken)}&email={WebUtility.UrlEncode(emailRes.Data.Value)}";

            // Send password reset email
            var emailSent = await _emailService.SendPasswordResetEmailAsync(user.Email.Value, resetToken, resetLink);
            
            if (!emailSent)
            {
                // Log the error but don't reveal it to the user for security
                // In production, you might want to log this to a monitoring system
                return MessageDTO.Success("Success", "If the email exists, a password reset link has been sent");
            }

            return MessageDTO.Success("Success", "If the email exists, a password reset link has been sent");
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
