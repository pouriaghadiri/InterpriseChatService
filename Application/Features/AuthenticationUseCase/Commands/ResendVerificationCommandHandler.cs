using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Repositories;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class ResendVerificationCommandHandler : IRequestHandler<ResendVerificationCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;

        public ResendVerificationCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
                return MessageDTO.Failure("User Not Found", null, "Invalid verification request");
            }

            // In a real implementation, you would:
            // 1. Check if email is already verified
            // 2. Generate a new verification token
            // 3. Send verification email
            
            return MessageDTO.Success("Success", "Verification email has been sent");
        }
    }
}
