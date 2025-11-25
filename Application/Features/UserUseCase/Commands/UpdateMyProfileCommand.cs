using Domain.Base;
using MediatR;

namespace Application.Features.UserUseCase.Commands
{
    /// <summary>
    /// Command to update current user's profile (uses JWT token for user ID)
    /// </summary>
    public class UpdateMyProfileCommand : IRequest<MessageDTO>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
    }
}

