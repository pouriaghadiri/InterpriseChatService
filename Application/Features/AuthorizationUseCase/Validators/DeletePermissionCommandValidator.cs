using FluentValidation;

namespace Application.Features.AuthorizationUseCase.Validators
{
    public class DeletePermissionCommandValidator : AbstractValidator<Commands.DeletePermissionCommand>
    {
        public DeletePermissionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Permission ID is required.");
        }
    }
}
