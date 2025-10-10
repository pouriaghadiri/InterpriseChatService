using Application.Features.RoleUseCase.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.RoleUseCase.Validators
{
    public class AddRoleCommandValidator : AbstractValidator<AddRoleCommand>
    {
        public AddRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("Role name is required.")
                .NotEmpty().WithMessage("Role name cannot be empty.")
                .When(x => x.Name != null)
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("Role name cannot be empty or whitespace.")
                .When(x => x.Name != null);

            RuleFor(x => x.Name)
                .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.")
                .When(x => x.Name != null && !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
