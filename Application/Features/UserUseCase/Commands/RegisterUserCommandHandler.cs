using Application.Common;
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
                return ResultDTO<Guid>.Failure("This email is already registered!");
            }
            PersonFullName personFullName = new PersonFullName(request.FirstName, request.LastName);
            Email email = new Email(request.Email);
            HashedPassword hashedPassword = new HashedPassword(request.Password);
            PhoneNumber phoneNumber = new PhoneNumber(request.PhoneNumber);

            var newUser = User.RegisterUser(personFullName, email, hashedPassword, phoneNumber, request.ProfilePicture, request.Bio, request.Location);

            // Fix: Remove the assignment to a variable since AddAsync returns void
            await _userRepository.AddAsync(newUser.Data);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResultDTO<Guid>.Success(newUser.Data.Id);
        }
    }
}
