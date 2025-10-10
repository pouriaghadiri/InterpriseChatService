﻿using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.DepartmentUseCase.Commands
{
    public class AddDepartmentCommandHandler : IRequestHandler<AddDepartmentCommand, MessageDTO>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddDepartmentCommandHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<MessageDTO> Handle(AddDepartmentCommand request, CancellationToken cancellationToken)
        {
            var departmentName = EntityName.Create(request.Name);
            if (!departmentName.IsSuccess)
            {
                return MessageDTO.Failure(departmentName.Title, departmentName.Errors, departmentName.Message);
            }
            var existDepartment = await _departmentRepository.ExistsAsync(x => x.Name.Value == departmentName.Data.Value, cancellationToken);
            if (existDepartment)
            {
                return MessageDTO.Failure("Exist Error", null, "This department already exists!");
            }


            var newDepartment = Department.CreateDepartment(departmentName.Data);
            if (!newDepartment.IsSuccess)
            {
                return MessageDTO.Failure(newDepartment.Title, newDepartment.Errors, newDepartment.Message);
            }

            await _departmentRepository.AddAsync(newDepartment.Data);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return MessageDTO.Success("Created", "Department added successfully.");
        }
    }
}
