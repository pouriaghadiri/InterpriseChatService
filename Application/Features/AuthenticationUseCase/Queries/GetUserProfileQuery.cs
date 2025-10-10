using Application.Features.AuthenticationUseCase.DTOs;
using Domain.Base;
using MediatR;

namespace Application.Features.AuthenticationUseCase.Queries
{
    public class GetUserProfileQuery : IRequest<ResultDTO<UserProfileDTO>>
    {
        // This query will get the current user's profile from the JWT token
    }
}
