using Application.Features.UserUseCase;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.PingUserCase
{
    public class PingCommandHandler : IRequestHandler<PingCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PingCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
         

        public Task<string> Handle(PingCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult("Pong");

        }
    }
}
