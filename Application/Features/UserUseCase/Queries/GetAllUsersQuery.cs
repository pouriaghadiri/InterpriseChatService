using Application.Features.UserUseCase.DTOs;
using Domain.Base;
using MediatR;

namespace Application.Features.UserUseCase.Queries
{
    public class GetAllUsersQuery : IRequest<ResultDTO<List<UserDTO>>>
    {
    }
}

