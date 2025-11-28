using FluentValidation;

namespace Application.Features.AuthenticationUseCase.Validators
{
    public class BlockTokenCommandValidator : AbstractValidator<Commands.BlockTokenCommand>
    {
        public BlockTokenCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotNull().WithMessage("Token is required.")
                .NotEmpty().WithMessage("Token cannot be empty.");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Reason));
        }
    }
}

