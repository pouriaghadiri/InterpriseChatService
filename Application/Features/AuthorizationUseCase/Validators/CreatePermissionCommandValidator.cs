using FluentValidation;

namespace Application.Features.AuthorizationUseCase.Validators
{
    public class CreatePermissionCommandValidator : AbstractValidator<Commands.CreatePermissionCommand>
    {
        public CreatePermissionCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("Permission name is required.")
                .NotEmpty().WithMessage("Permission name cannot be empty.")
                .MaximumLength(100).WithMessage("Permission name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotNull().WithMessage("Permission description is required.")
                .NotEmpty().WithMessage("Permission description cannot be empty.")
                .MaximumLength(500).WithMessage("Permission description must not exceed 500 characters.");
        }
    }
}
