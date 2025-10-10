using Application.Features.DepartmentUseCase.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.DepartmentUseCase.Validators
{
    public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Department ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Department ID cannot be empty.");

            RuleFor(x => x.Name)
                .NotNull().WithMessage("Department name is required.")
                .NotEmpty().WithMessage("Department name cannot be empty.")
                .When(x => x.Name != null)
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("Department name cannot be empty or whitespace.")
                .When(x => x.Name != null);

            RuleFor(x => x.Name)
                .MaximumLength(50).WithMessage("Department name must not exceed 50 characters.")
                .When(x => x.Name != null && !string.IsNullOrWhiteSpace(x.Name));
        }
    }
}
