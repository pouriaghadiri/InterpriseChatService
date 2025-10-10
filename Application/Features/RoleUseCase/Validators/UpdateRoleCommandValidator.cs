using Application.Features.RoleUseCase.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.RoleUseCase.Validators
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Role ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Role ID cannot be empty.");

            RuleFor(x => x.Name)
                .NotNull().WithMessage("Role name is required.")
                .NotEmpty().WithMessage("Role name cannot be empty.")
                .When(x => x.Name != null)
                .Must(name => !string.IsNullOrWhiteSpace(name?.Value))
                .WithMessage("Role name cannot be empty or whitespace.")
                .When(x => x.Name != null);

            RuleFor(x => x.Name.Value)
                .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.")
                .When(x => x.Name != null && !string.IsNullOrWhiteSpace(x.Name.Value));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
