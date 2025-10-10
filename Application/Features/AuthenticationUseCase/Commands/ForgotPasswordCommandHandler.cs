using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Repositories;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;

        public ForgotPasswordCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

            // In a real implementation, you would:
            // 1. Generate a password reset token
            // 2. Store it in the database with expiration
            // 3. Send an email with the reset link
            
            return MessageDTO.Success("Success", "If the email exists, a password reset link has been sent");
        }
    }
}
