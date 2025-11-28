using Application.Features.AuthenticationUseCase.Commands;
using Application.Features.AuthenticationUseCase.DTOs;
using Application.Features.AuthenticationUseCase.Services;
using Domain.Base;
using Domain.Base.Interface;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using RoleEntity = Domain.Entities.Role;
using DepartmentEntity = Domain.Entities.Department;
using Microsoft.AspNetCore.Http;

namespace Tests.Application.Authentication.Commands
{
    public class RefreshTokenCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IActiveDepartmentService> _activeDepartmentServiceMock;
        private readonly Mock<IUserRoleInDepartmentRepository> _userRoleInDepartmentRepositoryMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly RefreshTokenCommandHandler _handler;
        private readonly RefreshTokenCommand _request;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;

        public RefreshTokenCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _cacheServiceMock = new Mock<ICacheService>();
            _activeDepartmentServiceMock = new Mock<IActiveDepartmentService>();
            _userRoleInDepartmentRepositoryMock = new Mock<IUserRoleInDepartmentRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _handler = new RefreshTokenCommandHandler(
                _userRepositoryMock.Object,
                _jwtTokenServiceMock.Object,
                _cacheServiceMock.Object,
                _activeDepartmentServiceMock.Object,
                _userRoleInDepartmentRepositoryMock.Object,
                _refreshTokenRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _httpContextAccessor.Object);

            _request = new RefreshTokenCommand
            {
                RefreshToken = "valid-refresh-token"
            };
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenRefreshTokenIsEmpty()
        {
            // Arrange
            _request.RefreshToken = "";

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Invalid");
            result.Message.Should().Contain("Refresh token is required");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenTokenIsInvalid()
        {
            // Arrange
            _cacheServiceMock
                .Setup(service => service.GetAsync<object>(It.IsAny<string>()))
                .ReturnsAsync((object?)null);

            _jwtTokenServiceMock
                .Setup(service => service.ValidateToken(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((ClaimsPrincipal)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Auth");
            result.Message.Should().Contain("Invalid refresh token");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenTokenIsBlacklisted()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
            }));

            _cacheServiceMock
                .Setup(service => service.GetAsync<object>(It.IsAny<string>()))
                .ReturnsAsync((object?)null);

            _refreshTokenRepositoryMock
                .Setup(repo => repo.GetByTokenAsync(It.IsAny<string>()))
                .ReturnsAsync((RefreshToken?)null);

            _jwtTokenServiceMock
                .Setup(service => service.ValidateToken(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(principal);

            _jwtTokenServiceMock
                .Setup(service => service.GetUserIdFromToken(It.IsAny<string>()))
                .Returns(userId);

            _cacheServiceMock
                .Setup(service => service.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Auth");
            result.Message.Should().Contain("revoked");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
            }));

            var refreshTokenEntity = RefreshToken.Create(userId, "test-token", DateTime.Now.AddDays(7));

            _cacheServiceMock
                .Setup(service => service.GetAsync<object>(It.IsAny<string>()))
                .ReturnsAsync((object?)null);

            _refreshTokenRepositoryMock
                .Setup(repo => repo.GetByTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(refreshTokenEntity);

            _jwtTokenServiceMock
                .Setup(service => service.ValidateToken(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(principal);

            _jwtTokenServiceMock
                .Setup(service => service.GetUserIdFromToken(It.IsAny<string>()))
                .Returns(userId);

            _cacheServiceMock
                .Setup(service => service.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _userRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Title.Should().Be("Auth");
            result.Message.Should().Contain("User not found");
        }

        [Fact]
        public async Task Handle_Should_Return_Success_WhenTokenIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = CreateTestUser();
            var department = CreateTestDepartment();
            var role = CreateTestRole();
            var roles = new List<string> { "Admin" };
            var newToken = "new-access-token";
            var newRefreshToken = "new-refresh-token";
            var expireDate = DateTime.Now.AddHours(1);
            var refreshTokenExpireDate = DateTime.Now.AddDays(7);

            var refreshTokenEntity = RefreshToken.Create(userId, "valid-refresh-token", DateTime.Now.AddDays(7));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
            }));

            _cacheServiceMock
                .Setup(service => service.GetAsync<object>(It.IsAny<string>()))
                .ReturnsAsync((object?)null);

            _refreshTokenRepositoryMock
                .Setup(repo => repo.GetByTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(refreshTokenEntity);

            _jwtTokenServiceMock
                .Setup(service => service.ValidateToken(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(principal);

            _jwtTokenServiceMock
                .Setup(service => service.GetUserIdFromToken(It.IsAny<string>()))
                .Returns(userId);

            _cacheServiceMock
                .Setup(service => service.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _userRepositoryMock
                .Setup(repo => repo.GetbyIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);

            _activeDepartmentServiceMock
                .Setup(service => service.GetActiveDepartmentIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid?)null);

            _activeDepartmentServiceMock
                .Setup(service => service.SetActiveDepartmentIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _userRoleInDepartmentRepositoryMock
                .Setup(repo => repo.GetRolesOfUserInDepartment(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<RoleEntity> { role });

            DateTime capturedExpireDate = expireDate;
            _jwtTokenServiceMock
                .Setup(service => service.GenerateToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>(), out capturedExpireDate, null))
                .Returns(newToken);

            DateTime capturedRefreshTokenExpireDate = refreshTokenExpireDate;
            _jwtTokenServiceMock
                .Setup(service => service.GenerateRefreshToken(It.IsAny<User>(), out capturedRefreshTokenExpireDate))
                .Returns(newRefreshToken);

            _refreshTokenRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _refreshTokenRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _cacheServiceMock
                .Setup(service => service.SetAsync(It.IsAny<string>(), It.IsAny<TokenResultDTO>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            _cacheServiceMock
                .Setup(service => service.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            _cacheServiceMock
                .Setup(service => service.RemoveAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Token.Should().Be(newToken);
            result.Data.ExpireTime.Should().Be(expireDate);
            result.Message.Should().Contain("refreshed successfully");
        }

        private User CreateTestUser()
        {
            var fullName = PersonFullName.Create("John", "Doe").Data;
            var email = Email.Create("john@example.com").Data;
            var password = HashedPassword.Create("Test123!@#").Data;
            var phone = PhoneNumber.Create("09123456789").Data;
            var user = User.RegisterUser(fullName, email, password, phone, "profile.jpg", "Developer", "Tehran").Data;

            // Add a role with department
            var role = CreateTestRole();
            var department = CreateTestDepartment();
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                User = user,
                Role = role
            };
            userRole.UserRoleInDepartments = new List<UserRoleInDepartment>
            {
                new UserRoleInDepartment
                {
                    UserRoleId = userRole.Id,
                    DepartmentId = department.Id,
                    UserRole = userRole,
                    Department = department
                }
            };
            user.UserRoles = new List<UserRole> { userRole };

            return user;
        }

        private RoleEntity CreateTestRole()
        {
            var name = EntityName.Create("Admin").Data;
            return RoleEntity.CreateRole(name, "Administrator role").Data;
        }

        private DepartmentEntity CreateTestDepartment()
        {
            var name = EntityName.Create("IT").Data;
            return DepartmentEntity.CreateDepartment(name).Data;
        }
    }
}

