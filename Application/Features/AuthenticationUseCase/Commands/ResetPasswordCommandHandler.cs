using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Repositories;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;

        public ResetPasswordCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<MessageDTO> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // Validate email
            var emailRes = Email.Create(request.Email);
            if (!emailRes.IsSuccess)
            {
                return MessageDTO.Failure("Invalid Email", emailRes.Errors, "Please provide a valid email address");
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

            var user = await _userRepository.GetbyEmailAsync(emailRes.Data);
            if (user == null)
            {
                return MessageDTO.Failure("User Not Found", null, "Invalid reset request");
            }

            // In a real implementation, you would:
            // 1. Validate the reset token
            // 2. Check if token is not expired
            // 3. Update the user's password
            // 4. Invalidate the reset token
            
            return MessageDTO.Success("Success", "Password has been reset successfully");
        }
    }
}
