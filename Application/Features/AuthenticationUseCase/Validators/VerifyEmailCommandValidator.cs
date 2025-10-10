using FluentValidation;

namespace Application.Features.AuthenticationUseCase.Validators
{
    public class VerifyEmailCommandValidator : AbstractValidator<Commands.VerifyEmailCommand>
    {
        public VerifyEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email is required.")
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Please provide a valid email address.");

            RuleFor(x => x.Token)
                .NotNull().WithMessage("Verification token is required.")
                .NotEmpty().WithMessage("Verification token cannot be empty.");
        }
    }
}
