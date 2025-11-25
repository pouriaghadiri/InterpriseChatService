using Application.Features.UserUseCase.DTOs;
using Domain.Base;
using MediatR;

namespace Application.Features.UserUseCase.Queries
{
    public record GetUserByIdQuery(Guid Id) : IRequest<ResultDTO<UserDTO>>;
}

