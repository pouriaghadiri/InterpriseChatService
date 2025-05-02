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


        async Task<ResultDTO<Guid>> IRequestHandler<RegisterUserCommand, ResultDTO<Guid>>.Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var exist = await _userRepository.GetbyEmailAsync(new Email(request.Email));
            if (exist != null)
            {
                return ResultDTO<Guid>.Failure("Exist Error", null, "This email is already registered!");
            }
            PersonFullName personFullName = new PersonFullName(request.FirstName, request.LastName);
            Email email = new Email(request.Email);
            HashedPassword hashedPassword = new HashedPassword(request.Password);
            var phoneResult = PhoneNumber.Create(request.PhoneNumber);
            if (!phoneResult.IsSuccess)
                return ResultDTO<Guid>.Failure("Invalid input", phoneResult.Errors, "Phone number validation failed");

            var newUser = User.RegisterUser(personFullName, email, hashedPassword, phoneResult.Data, request.ProfilePicture, request.Bio, request.Location);
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
