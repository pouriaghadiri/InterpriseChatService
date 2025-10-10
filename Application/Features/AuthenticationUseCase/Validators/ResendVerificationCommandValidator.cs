using FluentValidation;

namespace Application.Features.AuthenticationUseCase.Validators
{
    public class ResendVerificationCommandValidator : AbstractValidator<Commands.ResendVerificationCommand>
    {
        public ResendVerificationCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email is required.")
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Please provide a valid email address.");
        }
    }
}
