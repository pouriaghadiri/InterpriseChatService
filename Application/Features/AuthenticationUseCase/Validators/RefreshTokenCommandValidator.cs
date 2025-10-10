using FluentValidation;

namespace Application.Features.AuthenticationUseCase.Validators
{
    public class RefreshTokenCommandValidator : AbstractValidator<Commands.RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotNull().WithMessage("Refresh token is required.")
                .NotEmpty().WithMessage("Refresh token cannot be empty.");
        }
    }
}
