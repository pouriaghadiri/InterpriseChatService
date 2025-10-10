using Domain.Base;
using Domain.Base.Interface;
using Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.DepartmentUseCase.Commands
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, MessageDTO>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDepartmentCommandHandler(IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MessageDTO> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var existingDepartment = await _departmentRepository.GetbyIdAsync(request.Id);
            if (existingDepartment == null)
            {
                return MessageDTO.Failure("Not Found", null, "Department not found.");
            }

            // Check if department is being used by any users
            var isDepartmentInUse = await _departmentRepository.IsDepartmentInUseAsync(request.Id, cancellationToken);
            if (isDepartmentInUse)
            {
                return MessageDTO.Failure("In Use Error", null, "Cannot delete department that has assigned users.");
            }

            await _departmentRepository.DeleteAsync(existingDepartment);
            await _unitOfWork.SaveChangesAsync();

            return MessageDTO.Success("Deleted", "Department deleted successfully.");
        }
    }
}
