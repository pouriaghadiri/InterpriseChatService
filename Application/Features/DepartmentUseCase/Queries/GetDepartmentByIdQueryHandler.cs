using Application.Features.DepartmentUseCase.DTOs;
using Domain.Base;
using Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.DepartmentUseCase.Queries
{
    public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, ResultDTO<DepartmentDTO>>
    {
        private readonly IDepartmentRepository _departmentRepository;

        public GetDepartmentByIdQueryHandler(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<ResultDTO<DepartmentDTO>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var department = await _departmentRepository.GetbyIdAsync(request.Id);
            
            if (department == null)
            {
                return ResultDTO<DepartmentDTO>.Failure("Not Found", null, "Department not found.");
            }

            var departmentDTO = new DepartmentDTO
            {
                Id = department.Id,
                Name = department.Name.Value,
                CreatedAt = department.CreatedAt,
                UpdatedAt = department.UpdatedAt
            };

            return ResultDTO<DepartmentDTO>.Success("Retrieved", departmentDTO, "Department retrieved successfully.");
        }
    }
}
