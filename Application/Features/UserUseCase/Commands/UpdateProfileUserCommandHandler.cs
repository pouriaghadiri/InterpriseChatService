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


        async Task<MessageDTO> IRequestHandler<UpdateProfileUserCommand, MessageDTO>.Handle(UpdateProfileUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetbyIdAsync(request.Id);
            if (user == null)
                return MessageDTO.Failure("NotFound Error", null, "This user is not valid!");
            
            PersonFullName personFullName = new PersonFullName(request.FirstName, request.LastName);
            PhoneNumber phoneNumber = new PhoneNumber(request.PhoneNumber);


            var result = user.UpdateUserProfile(personFullName, user.Email, phoneNumber, request.Bio, request.Location, request.ProfilePicture);
            if (!result.IsSuccess)
            {
                return result;
            }


            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MessageDTO.Success("Updated", "User profile Updated successfully.");

        }
    }
}
