using FluentValidation;

namespace Application.Features.AuthorizationUseCase.Validators
{
    public class UpdatePermissionCommandValidator : AbstractValidator<Commands.UpdatePermissionCommand>
    {
        public UpdatePermissionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Permission ID is required.");

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
