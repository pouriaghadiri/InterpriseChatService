using Application.Features.AuthorizationUseCase.DTOs;
using Domain.Base;
using MediatR;

namespace Application.Features.AuthorizationUseCase.Queries
{
    public class GetPermissionByIdQuery : IRequest<ResultDTO<PermissionDTO>>
    {
        public Guid Id { get; set; }

        public GetPermissionByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
