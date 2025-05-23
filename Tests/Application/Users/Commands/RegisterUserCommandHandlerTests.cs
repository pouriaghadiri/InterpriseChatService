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
using Xunit.Sdk;
using Domain.Base;

namespace Tests.UnitTest.Users.Commands
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly RegisterUserCommandHandler _handler;
        private readonly RegisterUserCommand _request;

        public RegisterUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new RegisterUserCommandHandler(_unitOfWorkMock.Object, _userRepositoryMock.Object);

            _request = new RegisterUserCommand
            {
                FirstName = "Ali",
                LastName = "Rezaei",
                Email = "ali@example.com",
                Password = "StrongPass123",
                PhoneNumber = "09123456789",
                Bio = "Developer",
                Location = "Tehran"
            };
        }

        [Fact]
        public async Task Handle_Should_RegisterUser_WhenInputIsValid()
        {
            // Arrange 

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
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBe(Guid.Empty);
            result.Message.Should().Be("User created successfully");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenEmailExist()
        {
            //Arrange
            var createdFullName = PersonFullName.Create(_request.FirstName, _request.LastName);
            PersonFullName personFullName = createdFullName.Data;

            var createdEmail = Email.Create(_request.Email);
            Email email = createdEmail.Data;

            var createdPassword = HashedPassword.Create(_request.Password);
            HashedPassword hashedPassword = createdPassword.Data;
            var phoneResult = PhoneNumber.Create(_request.PhoneNumber);
            var existUser = User.RegisterUser(personFullName, email, hashedPassword, phoneResult.Data, _request.ProfilePicture, _request.Bio, _request.Location);

            _userRepositoryMock.Setup(repo => repo.GetbyEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(existUser.Data);

            //Act
            var response = await _handler.Handle(_request, CancellationToken.None);

            //Assert
            response.Should().NotBeNull();
            response.IsSuccess.Should().BeFalse();
            response.Title.Should().BeSameAs("Exist Error");
        }
    }
}
