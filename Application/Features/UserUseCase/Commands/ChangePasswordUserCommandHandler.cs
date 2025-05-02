using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.UserUseCase.Commands
{
    public class ChangePasswordUserCommandHandler : IRequestHandler<ChangePasswordUserCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordUserCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }


        async Task<MessageDTO> IRequestHandler<ChangePasswordUserCommand, MessageDTO>.Handle(ChangePasswordUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetbyIdAsync(request.Id);
            if (user == null)
                return MessageDTO.Failure("This user is not valid!");
            
            if(!user.HashedPassword.Verify(request.CurrentPassword))
                return MessageDTO.Failure("The current password is incorrect.");
            
            if(request.NewPassword != request.NewPasswordConfirm)
                return MessageDTO.Failure("The new password and confirmation password do not match.");

            user.HashedPassword = HashedPassword.CreateFromPlain(request.NewPassword);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MessageDTO.Success("Password changed successfully.");

        }
    }
}
