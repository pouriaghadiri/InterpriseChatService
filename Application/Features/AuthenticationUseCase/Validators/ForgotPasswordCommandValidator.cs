using FluentValidation;

namespace Application.Features.AuthenticationUseCase.Validators
{
    public class ForgotPasswordCommandValidator : AbstractValidator<Commands.ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email is required.")
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Please provide a valid email address.");
        }
    }
}
