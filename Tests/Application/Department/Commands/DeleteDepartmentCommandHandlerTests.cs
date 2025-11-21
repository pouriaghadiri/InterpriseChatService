using Application.Features.DepartmentUseCase.Commands;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Department.Commands
{
    public class DeleteDepartmentCommandHandlerTests
    {
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeleteDepartmentCommandHandler _handler;
        private readonly DeleteDepartmentCommand _request;

        public DeleteDepartmentCommandHandlerTests()
        {
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new DeleteDepartmentCommandHandler(_unitOfWorkMock.Object, _departmentRepositoryMock.Object);

            _request = new DeleteDepartmentCommand
            {
                Id = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Handle_Should_DeleteDepartment_WhenInputIsValid()
        {
            // Arrange
            var existingDepartment = CreateTestDepartment();
            existingDepartment.Id = _request.Id;

            _departmentRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(existingDepartment);

            _departmentRepositoryMock
                .Setup(repo => repo.IsDepartmentInUseAsync(_request.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _departmentRepositoryMock
                .Setup(repo => repo.DeleteAsync(It.IsAny<Department>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Department deleted successfully.");
            _departmentRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Department>()), Times.Once);
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
        public async Task Handle_Should_Return_Failure_WhenDepartmentIsInUse()
        {
            // Arrange
            var existingDepartment = CreateTestDepartment();
            existingDepartment.Id = _request.Id;

            _departmentRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(_request.Id))
                .ReturnsAsync(existingDepartment);

            _departmentRepositoryMock
                .Setup(repo => repo.IsDepartmentInUseAsync(_request.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("In Use Error");
            result.Message.Should().Contain("has assigned users");
        }

        private Department CreateTestDepartment()
        {
            var name = EntityName.Create("IT").Data;
            return Department.CreateDepartment(name).Data;
        }
    }
}

