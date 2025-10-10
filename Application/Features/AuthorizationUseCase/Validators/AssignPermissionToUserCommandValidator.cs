using FluentValidation;

namespace Application.Features.AuthorizationUseCase.Validators
{
    public class AssignPermissionToUserCommandValidator : AbstractValidator<Commands.AssignPermissionToUserCommand>
    {
        public AssignPermissionToUserCommandValidator()
        {
            RuleFor(x => x.PermissionId)
                .NotEmpty().WithMessage("Permission ID is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department ID is required.");
        }
    }
}
