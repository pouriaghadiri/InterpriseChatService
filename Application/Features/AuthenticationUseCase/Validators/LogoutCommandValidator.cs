using FluentValidation;

namespace Application.Features.AuthenticationUseCase.Validators
{
    public class LogoutCommandValidator : AbstractValidator<Commands.LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            // Logout command doesn't require validation as it's a simple operation
        }
    }
}
