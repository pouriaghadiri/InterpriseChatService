using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.UserUseCase.Commands
{
    public class UpdateProfileUserCommandHandler : IRequestHandler<UpdateProfileUserCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfileUserCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MessageDTO> Handle(UpdateProfileUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetbyIdAsync(request.Id);
            if (user == null)
                return MessageDTO.Failure("NotFound Error", null, "This user is not valid!");

            var createdFullName = PersonFullName.Create(request.FirstName, request.LastName);
            if (!createdFullName.IsSuccess || createdFullName.Data == null)
            {
                return MessageDTO.Failure("Invalid input", null, "Fullname validation failed");
            }
            PersonFullName personFullName = createdFullName.Data;

            var phoneResult = PhoneNumber.Create(request.PhoneNumber);
            if (!phoneResult.IsSuccess)
                return MessageDTO.Failure("Invalid input", phoneResult.Errors, "Phone number validation failed!");

            var result = user.UpdateUserProfile(personFullName, user.Email, phoneResult.Data, request.Bio, request.Location, request.ProfilePicture);
            if (!result.IsSuccess)
            {
                return result;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MessageDTO.Success("Updated", "User profile Updated successfully.");
        }
    }
}
