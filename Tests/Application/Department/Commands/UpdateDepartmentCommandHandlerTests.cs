using Application.Features.DepartmentUseCase.Commands;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Department.Commands
{
    public class UpdateDepartmentCommandHandlerTests
    {
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateDepartmentCommandHandler _handler;
        private readonly UpdateDepartmentCommand _request;

        public UpdateDepartmentCommandHandlerTests()
        {
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateDepartmentCommandHandler(_unitOfWorkMock.Object, _departmentRepositoryMock.Object);

            _request = new UpdateDepartmentCommand
            {
                Id = Guid.NewGuid(),
                Name = "UpdatedIT"
            };
        }

        [Fact]
        public async Task Handle_Should_UpdateDepartment_WhenInputIsValid()
        {
            // Arrange
            var existingDepartment = CreateTestDepartment();
            existingDepartment.Id = _request.Id;

            _departmentRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(existingDepartment);

            _departmentRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Department, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _departmentRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Department>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Department updated successfully.");
            _departmentRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Department>()), Times.Once);
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

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenDepartmentNameAlreadyExists()
        {
            // Arrange
            var existingDepartment = CreateTestDepartment();
            existingDepartment.Id = _request.Id;

            _departmentRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(existingDepartment);

            _departmentRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Department, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Exist Error");
            result.Message.Should().Contain("already exists");
        }

        private Department CreateTestDepartment()
        {
            var name = EntityName.Create("IT").Data;
            return Department.CreateDepartment(name).Data;
        }
    }
}

