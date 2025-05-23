using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.UserUseCase.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ResultDTO<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<ResultDTO<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            if (!email.IsSuccess || email.Data == null)
            {
                return ResultDTO<Guid>.Failure("Invalid input", email.Errors, "Email validation failed");
            }
            var exist = await _userRepository.GetbyEmailAsync(email.Data);
            if (exist != null)
            {
                return ResultDTO<Guid>.Failure("Exist Error", null, "This email is already registered!");
            }
            var person = PersonFullName.Create(request.FirstName, request.LastName);
            if (!person.IsSuccess || person.Data == null)
            { 
                return ResultDTO<Guid>.Failure("Invalid input", person.Errors, "Full Name validation failed");
            }
            PersonFullName personFullName = person.Data;
            var password = HashedPassword.Create(request.Password);
            if (!password.IsSuccess || password.Data == null)
            {
                return ResultDTO<Guid>.Failure("Invalid input", password.Errors, "Password validation failed");
            }

            HashedPassword hashedPassword = password.Data;
            var phoneResult = PhoneNumber.Create(request.PhoneNumber);
            if (!phoneResult.IsSuccess)
                return ResultDTO<Guid>.Failure("Invalid input", phoneResult.Errors, "Phone number validation failed");

            var newUser = User.RegisterUser(personFullName, email.Data, hashedPassword, phoneResult.Data, request.ProfilePicture, request.Bio, request.Location);
            if (!newUser.IsSuccess)
            {
                return ResultDTO<Guid>.Failure(newUser.Title, null, newUser.Message);
            }
            // Fix: Remove the assignment to a variable since AddAsync returns void
            await _userRepository.AddAsync(newUser.Data);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResultDTO<Guid>.Success("Created", newUser.Data.Id, "User created successfully");
        }
    }
}
