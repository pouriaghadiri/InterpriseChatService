using Application.Features.DepartmentUseCase.DTOs;
using Domain.Base;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.DepartmentUseCase.Queries
{
    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, ResultDTO<List<DepartmentDTO>>>
    {
        private readonly IDepartmentRepository _departmentRepository;

        public GetAllDepartmentsQueryHandler(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<ResultDTO<List<DepartmentDTO>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var departments = await _departmentRepository.GetAllAsync();
            
            if (departments == null || !departments.Any())
            {
                return ResultDTO<List<DepartmentDTO>>.Success("Retrieved", new List<DepartmentDTO>(), "No departments found.");
            }

            var departmentDTOs = departments.Select(department => new DepartmentDTO
            {
                Id = department.Id,
                Name = department.Name.Value,
                CreatedAt = department.CreatedAt,
                UpdatedAt = department.UpdatedAt
            }).ToList();

            return ResultDTO<List<DepartmentDTO>>.Success("Retrieved", departmentDTOs, "Departments retrieved successfully.");
        }
    }
}
