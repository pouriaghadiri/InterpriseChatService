﻿using Application.Features.UserUseCase.Commands;
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
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^(09\d{9}|\+989\d{9})$")
                .WithMessage("Phone number must start with 09 or +989 and be followed by 9 digits."); // E.164 pattern

            RuleFor(x => x.ProfilePicture)
                .MaximumLength(200).WithMessage("Profile picture URL must not exceed 200 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.ProfilePicture));

            RuleFor(x => x.Bio)
                .MaximumLength(500).WithMessage("Bio must not exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Bio));

            RuleFor(x => x.Location)
                .MaximumLength(100).WithMessage("Location must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Location));
        }
    }
}
