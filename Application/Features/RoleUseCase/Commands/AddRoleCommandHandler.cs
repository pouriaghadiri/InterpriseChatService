using Application.Features.DepartmentUseCase.Commands;
using Domain.Base.Interface;
using Domain.Base;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.RoleUseCase.Commands
{
    class AddRoleCommandHandler : IRequestHandler<AddRoleCommand, MessageDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddRoleCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MessageDTO> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
