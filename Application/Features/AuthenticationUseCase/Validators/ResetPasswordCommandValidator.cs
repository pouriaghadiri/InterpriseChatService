using FluentValidation;

namespace Application.Features.AuthenticationUseCase.Validators
{
    public class ResetPasswordCommandValidator : AbstractValidator<Commands.ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email is required.")
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Please provide a valid email address.");

            RuleFor(x => x.Token)
                .NotNull().WithMessage("Reset token is required.")
                .NotEmpty().WithMessage("Reset token cannot be empty.");

            RuleFor(x => x.NewPassword)
                .NotNull().WithMessage("New password is required.")
                .NotEmpty().WithMessage("New password cannot be empty.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");

            RuleFor(x => x.ConfirmPassword)
                .NotNull().WithMessage("Password confirmation is required.")
                .NotEmpty().WithMessage("Password confirmation cannot be empty.")
                .Equal(x => x.NewPassword).WithMessage("Password and confirmation must match.");
        }
    }
}
