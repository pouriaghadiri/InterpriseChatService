using Application.Features.UserUseCase.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserUseCase.Validators
{
    public class RegisterUserCommandValidator: AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .Length(2, 50)
                .WithMessage("First name must be between 2 and 50 characters.");
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .Length(2, 50)
                .WithMessage("Last name must be between 2 and 50 characters.");
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long.");
        }
    }
}
