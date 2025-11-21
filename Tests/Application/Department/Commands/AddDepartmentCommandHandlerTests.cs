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
    public class AddDepartmentCommandHandlerTests
    {
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly AddDepartmentCommandHandler _handler;
        private readonly AddDepartmentCommand _request;

        public AddDepartmentCommandHandlerTests()
        {
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new AddDepartmentCommandHandler(_departmentRepositoryMock.Object, _unitOfWorkMock.Object);

            _request = new AddDepartmentCommand
            {
                Name = "IT"
            };
        }

        [Fact]
        public async Task Handle_Should_CreateDepartment_WhenInputIsValid()
        {
            // Arrange
            _departmentRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Department, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _departmentRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Department>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Department added successfully.");
            _departmentRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Department>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenDepartmentNameIsInvalid()
        {
            // Arrange
            _request.Name = "";

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenDepartmentNameAlreadyExists()
        {
            // Arrange
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
    }
}

