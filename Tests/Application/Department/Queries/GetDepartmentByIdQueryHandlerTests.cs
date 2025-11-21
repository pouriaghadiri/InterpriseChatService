using Application.Features.DepartmentUseCase.DTOs;
using Application.Features.DepartmentUseCase.Queries;
using Domain.Base;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Department.Queries
{
    public class GetDepartmentByIdQueryHandlerTests
    {
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly GetDepartmentByIdQueryHandler _handler;
        private readonly GetDepartmentByIdQuery _request;

        public GetDepartmentByIdQueryHandlerTests()
        {
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _handler = new GetDepartmentByIdQueryHandler(_departmentRepositoryMock.Object);
            _request = new GetDepartmentByIdQuery
            {
                Id = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Handle_Should_Return_Department_WhenFound()
        {
            // Arrange
            var department = CreateTestDepartment();
            department.Id = _request.Id;

            _departmentRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(department);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(_request.Id);
            result.Data.Name.Should().Be("IT");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenDepartmentNotFound()
        {
            // Arrange
            _departmentRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync((Department?)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Not Found");
            result.Message.Should().Be("Department not found.");
        }

        private Department CreateTestDepartment()
        {
            var name = EntityName.Create("IT").Data;
            return Department.CreateDepartment(name).Data;
        }
    }
}

