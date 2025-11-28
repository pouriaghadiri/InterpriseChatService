using FluentValidation;

namespace Application.Features.AuthenticationUseCase.Validators
{
    public class BlockTokenCommandValidator : AbstractValidator<Commands.BlockTokenCommand>
    {
        public BlockTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotNull().WithMessage("Refresh token is required.")
                .NotEmpty().WithMessage("Refresh token cannot be empty.");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Reason));
        }
    }
}

