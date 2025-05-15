using Application.Features.UserUseCase.Commands;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Tests.UnitTest.Users.Commands
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new RegisterUserCommandHandler(_unitOfWorkMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_RegisterUser_WhenInputIsValid()
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                FirstName = "Ali",
                LastName = "Rezaei",
                Email = "ali@example.com",
                Password = "StrongPass123",
                PhoneNumber = "09123456789",
                Bio = "Developer",
                Location = "Tehran"
            };

            _userRepositoryMock
                .Setup(repo => repo.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync((User?)null); // ایمیل قبلا ثبت نشده

            _userRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask); // اضافه‌کردن کاربر به درستی انجام می‌شود

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1); // ذخیره موفق

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBe(Guid.Empty);
            result.Message.Should().Be("User created successfully");
        }
    }
}
