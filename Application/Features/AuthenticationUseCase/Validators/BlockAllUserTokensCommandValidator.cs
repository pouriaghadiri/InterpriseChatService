using FluentValidation;

namespace Application.Features.AuthenticationUseCase.Validators
{
    public class BlockAllUserTokensCommandValidator : AbstractValidator<Commands.BlockAllUserTokensCommand>
    {
        public BlockAllUserTokensCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Reason));
        }
    }
}

