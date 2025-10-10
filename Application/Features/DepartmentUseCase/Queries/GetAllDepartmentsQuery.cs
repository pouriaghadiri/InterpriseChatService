using Application.Features.DepartmentUseCase.DTOs;
using Domain.Base;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.DepartmentUseCase.Queries
{
    public class GetAllDepartmentsQuery : IRequest<ResultDTO<List<DepartmentDTO>>>
    {
    }
}
