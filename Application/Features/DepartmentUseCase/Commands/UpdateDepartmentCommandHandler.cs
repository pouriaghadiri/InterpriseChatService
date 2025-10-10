using Domain.Base;
using Domain.Base.Interface;
using Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.DepartmentUseCase.Commands
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, MessageDTO>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MessageDTO> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var existingDepartment = await _departmentRepository.GetbyIdAsync(request.Id);
            if (existingDepartment == null)
            {
                return MessageDTO.Failure("Not Found", null, "Department not found.");
            }

            // Check if another department with the same name exists (excluding current department)
            var departmentName = Domain.Common.ValueObjects.EntityName.Create(request.Name);
            if (!departmentName.IsSuccess)
            {
                return MessageDTO.Failure(departmentName.Title, departmentName.Errors, departmentName.Message);
            }
            
            var departmentWithSameName = await _departmentRepository.ExistsAsync(x => x.Name == departmentName.Data && x.Id != request.Id, cancellationToken);
            if (departmentWithSameName)
            {
                return MessageDTO.Failure("Exist Error", null, "A department with this name already exists.");
            }

            // Update the department properties
            existingDepartment.Name = departmentName.Data;

            await _departmentRepository.UpdateAsync(existingDepartment);
            await _unitOfWork.SaveChangesAsync();

            return MessageDTO.Success("Updated", "Department updated successfully.");
        }
    }
}
