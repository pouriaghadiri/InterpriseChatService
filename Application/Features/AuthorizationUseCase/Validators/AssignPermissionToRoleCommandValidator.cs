using FluentValidation;

namespace Application.Features.AuthorizationUseCase.Validators
{
    public class AssignPermissionToRoleCommandValidator : AbstractValidator<Commands.AssignPermissionToRoleCommand>
    {
        public AssignPermissionToRoleCommandValidator()
        {
            RuleFor(x => x.PermissionId)
                .NotEmpty().WithMessage("Permission ID is required.");

            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role ID is required.");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department ID is required.");
        }
    }
}
