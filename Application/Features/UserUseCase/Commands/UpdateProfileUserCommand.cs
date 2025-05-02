using Domain.Base;
using Domain.Common.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserUseCase.Commands
{
    public class UpdateProfileUserCommand : IRequest<MessageDTO>
    {
        public Guid Id{ get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!; 
        public string PhoneNumber { get; set; } = default!;
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }

    }
}
