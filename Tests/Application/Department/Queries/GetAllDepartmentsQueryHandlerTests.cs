using Application.Features.DepartmentUseCase.DTOs;
using Application.Features.DepartmentUseCase.Queries;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using DepartmentEntity = Domain.Entities.Department;

namespace Tests.Application.Department.Queries
{
    public class GetAllDepartmentsQueryHandlerTests
    {
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly GetAllDepartmentsQueryHandler _handler;
        private readonly GetAllDepartmentsQuery _request;

        public GetAllDepartmentsQueryHandlerTests()
        {
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _handler = new GetAllDepartmentsQueryHandler(_departmentRepositoryMock.Object);
            _request = new GetAllDepartmentsQuery();
        }

        [Fact]
        public async Task Handle_Should_Return_AllDepartments()
        {
            // Arrange
            var departments = new List<DepartmentEntity>
            {
                CreateTestDepartment("IT"),
                CreateTestDepartment("HR"),
                CreateTestDepartment("Finance")
            };

            _departmentRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(departments);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Count.Should().Be(3);
            result.Data[0].Name.Should().Be("IT");
        }

        [Fact]
        public async Task Handle_Should_Return_EmptyList_WhenNoDepartments()
        {
            // Arrange
            _departmentRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<DepartmentEntity>());

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEmpty();
        }

        private DepartmentEntity CreateTestDepartment(string name)
        {
            var entityName = EntityName.Create(name).Data;
            return DepartmentEntity.CreateDepartment(entityName).Data;
        }
    }
}

