using Application.Features.DepartmentUseCase.DTOs;
using Domain.Base;
using MediatR;
using System;

namespace Application.Features.DepartmentUseCase.Queries
{
    public class GetDepartmentByIdQuery : IRequest<ResultDTO<DepartmentDTO>>
    {
        public Guid Id { get; set; }

        public GetDepartmentByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
