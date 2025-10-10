using Application.Features.RoleUseCase.DTOs;
using Domain.Base;
using MediatR;
using System;

namespace Application.Features.RoleUseCase.Queries
{
    public class GetRoleByIdQuery : IRequest<ResultDTO<RoleDTO>>
    {
        public Guid Id { get; set; }

        public GetRoleByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
