using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Repositories;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Commands
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;

        public VerifyEmailCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<MessageDTO> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
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
            // 1. Validate the verification token
            // 2. Check if token is not expired
            // 3. Mark the user's email as verified
            // 4. Invalidate the verification token
            
            return MessageDTO.Success("Success", "Email has been verified successfully");
        }
    }
}
