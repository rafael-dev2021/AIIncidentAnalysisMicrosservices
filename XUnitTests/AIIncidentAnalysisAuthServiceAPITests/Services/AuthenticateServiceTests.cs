using System.Security.Claims;
using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Exceptions;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Services;
using AIIncidentAnalysisAuthServiceAPI.Services.Interfaces;
using AutoMapper;
using Moq;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Services;

public class AuthenticateServiceTests
{
    private readonly Mock<IAuthRepository> _repositoryMock;
    private readonly AuthenticateService _authenticateService;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ITokenManagerService> _tokenManagerServiceMock;


    protected AuthenticateServiceTests()
    {
        _repositoryMock = new Mock<IAuthRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _mapperMock = new Mock<IMapper>();
        _tokenManagerServiceMock = new Mock<ITokenManagerService>();
        _authenticateService = new AuthenticateService(
            _repositoryMock.Object,
            _tokenServiceMock.Object,
            _mapperMock.Object,
            _tokenManagerServiceMock.Object
        );
    }

    public class GetAllUsersDtoAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task GetAllUsersDtoAsync_ShouldReturnMappedUserDtos_WhenUsersExist()
        {
            // Arrange
            var user1 = new PoliceOfficer { Id = "1", Email = "user1@example.com" };
            user1.SetName("Name 1");
            user1.SetLastName("Last Name 1");
            user1.SetRole("Admin");

            var user2 = new PoliceOfficer { Id = "2", Email = "user2@example.com" };
            user2.SetName("Name 2");
            user2.SetLastName("Last Name 2");
            user2.SetRole("User");

            var users = new List<PoliceOfficer>
            {
                user1,
                user2
            };

            var userDtos = new List<UserDtoResponse>
            {
                new(
                    "1",
                    "RG2451",
                    "John Doe",
                    "John Doe",
                    "RT2445",
                    "Admin",
                    DateTime.Now.AddYears(25),
                    DateTime.Now.AddYears(-5),
                    "Sergeant",
                    "TrafficDivision",
                    "Active",
                    "ReadWrite"
                ),

                new(
                    "2",
                    "RG24451",
                    "John Doe",
                    "John Doe",
                    "RT24f45",
                    "Admin",
                    DateTime.Now.AddYears(25),
                    DateTime.Now.AddYears(-5),
                    "Sergeant",
                    "TrafficDivision",
                    "Active",
                    "ReadWrite"
                )
            };

            _repositoryMock.Setup(r => r.ListPoliceOfficersAsync()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserDtoResponse>>(users)).Returns(userDtos);

            // Act
            var result = await _authenticateService.ListAllUsersServiceAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDtos, result);
        }

        [Fact]
        public async Task GetAllUsersDtoAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var users = new List<PoliceOfficer>();

            _repositoryMock.Setup(r => r.ListPoliceOfficersAsync()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserDtoResponse>>(users)).Returns(new List<UserDtoResponse>());

            // Act
            var result = await _authenticateService.ListAllUsersServiceAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }

    public class LoginAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task LoginAsync_ShouldReturnTokenDtoResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginDtoRequest("test@example.com", "password", true);
            var authResponse = new AuthDtoResponse(true, "Success");
            var user = new PoliceOfficer { Email = "test@example.com", Id = "1" };
            user.SetName("Test");
            user.SetLastName("Test");
            user.SetCpf("131.515.555-12");
            var tokenResponse = new TokenDtoResponse("accessToken", "refreshToken");

            _repositoryMock.Setup(r => r.AuthenticateAsync(It.IsAny<LoginDtoRequest>()))
                .ReturnsAsync(authResponse);
            _repositoryMock.Setup(r => r.GetUserProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _tokenManagerServiceMock.Setup(tms => tms.GenerateTokenResponseAsync(It.IsAny<PoliceOfficer>()))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _authenticateService.LoginServiceAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tokenResponse.Token, result.Token);
            Assert.Equal(tokenResponse.RefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginRequest = new LoginDtoRequest("test@example.com", "wrongpassword", true);
            var authResponse = new AuthDtoResponse(false, "Invalid credentials");

            _repositoryMock.Setup(r => r.AuthenticateAsync(It.IsAny<LoginDtoRequest>()))
                .ReturnsAsync(authResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authenticateService.LoginServiceAsync(loginRequest)
            );
            Assert.Equal("Invalid credentials", exception.Message);
        }
    }

    public class RegisterAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task RegisterAsync_ShouldReturnTokenDtoResponse_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var request = new RegisterDtoRequest(
                "ReadWrite",
                "ReadWrite",
                "readwrite@localhost.com",
                "+5512345681",
                "123.456.789-13",
                new DateTime(2000, 2, 15),
                DateTime.Now.AddYears(-2),
                ERank.Sergeant,
                EDepartment.TrafficDivision,
                EOfficerStatus.Active,
                EAccessLevel.ReadWrite,
                "@Visual24k+",
                "@Visual24k+"
            );

            var registerResponse = new RegisterDtoResponse(true, "Registration successful.");
            var user = new PoliceOfficer { Email = "test@example.com", Id = "1" };
            user.SetName("Test");
            user.SetLastName("Test");
            user.SetCpf("131.515.555-12");
            var tokenResponse = new TokenDtoResponse("accessToken", "refreshToken");

            _repositoryMock.Setup(r => r.RegisterAsync(It.IsAny<RegisterDtoRequest>()))
                .ReturnsAsync(registerResponse);
            _repositoryMock.Setup(r => r.GetUserProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _tokenManagerServiceMock.Setup(tms => tms.GenerateTokenResponseAsync(It.IsAny<PoliceOfficer>()))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _authenticateService.RegisterServiceAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tokenResponse.Token, result.Token);
            Assert.Equal(tokenResponse.RefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowUnauthorizedAccessException_WhenRegistrationFails()
        {
            // Arrange
            var request = new RegisterDtoRequest(
                "ReadWrite",
                "ReadWrite",
                "readwrite@localhost.com",
                "+5512345681",
                "123.456.789-13",
                new DateTime(2000, 2, 15),
                DateTime.Now.AddYears(-2),
                ERank.Sergeant,
                EDepartment.TrafficDivision,
                EOfficerStatus.Active,
                EAccessLevel.ReadWrite,
                "@Visual24k+",
                "@Visual24k+"
            );
            var registerResponse = new RegisterDtoResponse(false, "Registration failed.");

            _repositoryMock.Setup(r => r.RegisterAsync(It.IsAny<RegisterDtoRequest>()))
                .ReturnsAsync(registerResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authenticateService.RegisterServiceAsync(request)
            );
            Assert.Equal("Registration failed.", exception.Message);
        }
    }

    public class UpdateAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task UpdateAsync_ShouldReturnTokenDtoResponse_WhenUpdateIsSuccessful()
        {
            // Arrange
            var updateRequest = new UpdateUserDtoRequest(
                "NewName", "NewLastName", "1234567890", "newemail@example.com");
            var updateResponse = new UpdateDtoResponse(true, "Profile updated successfully.");
            var user = new PoliceOfficer { Email = "newemail@example.com", Id = "1" };
            user.SetName("NewName");
            user.SetLastName("NewLastName");
            user.SetCpf("131.515.555-12");
            var tokenResponse = new TokenDtoResponse("accessToken", "refreshToken");

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<UpdateUserDtoRequest>(), It.IsAny<string>()))
                .ReturnsAsync(updateResponse);
            _repositoryMock.Setup(r => r.GetUserProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _tokenManagerServiceMock.Setup(tms => tms.GenerateTokenResponseAsync(It.IsAny<PoliceOfficer>()))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _authenticateService.UpdateUserServiceAsync(updateRequest, user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tokenResponse.Token, result.Token);
            Assert.Equal(tokenResponse.RefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowUnauthorizedAccessException_WhenUpdateFails()
        {
            // Arrange
            var updateRequest = new UpdateUserDtoRequest(
                "NewName", "NewLastName", "1234567890", "newemail@example.com");
            var updateResponse = new UpdateDtoResponse(false, "Failed to update profile.");

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<UpdateUserDtoRequest>(), It.IsAny<string>()))
                .ReturnsAsync(updateResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authenticateService.UpdateUserServiceAsync(updateRequest, "1")
            );
            Assert.Equal("Failed to update profile.", exception.Message);
        }
    }

    public class ChangePasswordAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnTrue_WhenPasswordChangeIsSuccessful()
        {
            // Arrange
            var user = new PoliceOfficer { Email = "test@example.com", Id = "1" };
            var changePasswordRequest = new ChangePasswordDtoRequest("test@example.com", "oldPassword", "newPassword");

            _repositoryMock.Setup(r => r.GetUserIdProfileAsync(It.IsAny<string>())).ReturnsAsync(user);
            _repositoryMock.Setup(r => r.ChangePasswordAsync(It.IsAny<ChangePasswordDtoRequest>())).ReturnsAsync(true);

            // Act
            var result = await _authenticateService.ChangePasswordServiceAsync(changePasswordRequest, user.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrowUnauthorizedAccessException_WhenUserNotFound()
        {
            // Arrange
            var changePasswordRequest = new ChangePasswordDtoRequest("test@example.com", "oldPassword", "newPassword");

            _repositoryMock.Setup(r => r.GetUserIdProfileAsync(It.IsAny<string>())).ReturnsAsync((PoliceOfficer)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authenticateService.ChangePasswordServiceAsync(changePasswordRequest, "1")
            );
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrowUnauthorizedAccessException_WhenEmailDoesNotMatch()
        {
            // Arrange
            var user = new PoliceOfficer { Email = "different@example.com", Id = "1" };
            user.SetName("NewName");
            user.SetLastName("NewLastName");
            user.SetCpf("131.515.555-12");

            var changePasswordRequest = new ChangePasswordDtoRequest("test@example.com", "oldPassword", "newPassword");

            _repositoryMock.Setup(r => r.GetUserIdProfileAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authenticateService.ChangePasswordServiceAsync(changePasswordRequest, user.Id)
            );
            Assert.Equal("Unauthorized attempt to change password", exception.Message);
        }
    }

    public class LogoutAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task LogoutAsync_ShouldCallRepositoryAndLog()
        {
            // Arrange
            _repositoryMock.Setup(r => r.LogoutAsync()).Returns(Task.CompletedTask);

            // Act
            await _authenticateService.LogoutServiceAsync();

            // Assert
            _repositoryMock.Verify(
                r => r.LogoutAsync(),
                Times.Once);
        }
    }

    public class ForgotPasswordAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task ForgotPasswordAsync_ShouldCallRepositoryAndLog()
        {
            // Arrange
            var request = new ForgotPasswordDtoRequest("test@example.com", "newPassword");

            _repositoryMock.Setup(r => r.ForgotPasswordAsync(request)).ReturnsAsync(true);

            // Act
            var result = await _authenticateService.ForgotPasswordServiceAsync(request);

            // Assert
            Assert.True(result);

            _repositoryMock.Verify(
                r => r.ForgotPasswordAsync(request),
                Times.Once);
        }
    }

    public class RefreshTokenAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedAccessException_WhenRefreshTokenIsInvalid()
        {
            // Arrange
            var request = new RefreshTokenDtoRequest("validToken");
            _tokenServiceMock.Setup(ts => ts.ValidateToken(It.IsAny<string>())).Returns((ClaimsPrincipal)null!);

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<TokenRefreshException>(() =>
                    _authenticateService.RefreshTokenServiceAsync(request));
            Assert.Equal("[REFRESH_TOKEN] Error processing token refresh request.", exception.Message);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedAccessException_WhenEmailClaimIsNotFound()
        {
            // Arrange
            var request = new RefreshTokenDtoRequest("validToken");
            var claims = new List<Claim>
            {
                Capacity = 0
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _tokenServiceMock.Setup(ts => ts.ValidateToken(It.IsAny<string>())).Returns(claimsPrincipal);

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<TokenRefreshException>(() =>
                    _authenticateService.RefreshTokenServiceAsync(request));
            Assert.Equal("[REFRESH_TOKEN] Error processing token refresh request.", exception.Message);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotFound()
        {
            // Arrange
            var request = new RefreshTokenDtoRequest("validToken");
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.Email, "test@example.com")
            }));
            _tokenServiceMock.Setup(ts => ts.ValidateToken(It.IsAny<string>())).Returns(claimsPrincipal);
            _repositoryMock.Setup(r => r.GetUserProfileAsync(It.IsAny<string>())).ReturnsAsync((PoliceOfficer)null!);

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<TokenRefreshException>(() =>
                    _authenticateService.RefreshTokenServiceAsync(request));
            Assert.Equal("[REFRESH_TOKEN] Error processing token refresh request.", exception.Message);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnTokenDtoResponse_WhenRefreshTokenIsValid()
        {
            // Arrange
            var request = new RefreshTokenDtoRequest("validToken");
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.Email, "test@example.com")
            }));
            var user = new PoliceOfficer { Email = "test@example.com", Id = "1" };
            user.SetName("Test");
            user.SetLastName("User");
            var tokenResponse = new TokenDtoResponse("newAccessToken", "newRefreshToken");

            _tokenServiceMock.Setup(ts => ts.ValidateToken(It.IsAny<string>())).Returns(claimsPrincipal);
            _repositoryMock.Setup(r => r.GetUserProfileAsync(It.IsAny<string>())).ReturnsAsync(user);
            _tokenManagerServiceMock.Setup(tms => tms.GenerateTokenResponseAsync(It.IsAny<PoliceOfficer>()))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _authenticateService.RefreshTokenServiceAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newAccessToken", result.Token);
            Assert.Equal("newRefreshToken", result.RefreshToken);
        }
    }

    public class RevokedTokenAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task RevokedTokenAsync_ShouldReturnTrue_WhenTokenIsRevoked()
        {
            // Arrange
            const string tokenValue = "revokedToken";
            _tokenManagerServiceMock.Setup(tms => tms.RevokedTokenAsync(tokenValue)).ReturnsAsync(true);

            // Act
            var result = await _authenticateService.RevokedTokenServiceAsync(tokenValue);

            // Assert
            Assert.True(result);
            _tokenManagerServiceMock.Verify(tms => tms.RevokedTokenAsync(tokenValue), Times.Once);
        }

        [Fact]
        public async Task RevokedTokenAsync_ShouldReturnFalse_WhenTokenIsNotRevoked()
        {
            // Arrange
            const string tokenValue = "validToken";
            _tokenManagerServiceMock.Setup(tms => tms.RevokedTokenAsync(tokenValue)).ReturnsAsync(false);

            // Act
            var result = await _authenticateService.RevokedTokenServiceAsync(tokenValue);

            // Assert
            Assert.False(result);
            _tokenManagerServiceMock.Verify(tms => tms.RevokedTokenAsync(tokenValue), Times.Once);
        }
    }

    public class ExpiredTokenAsyncTests : AuthenticateServiceTests
    {
        [Fact]
        public async Task ExpiredTokenAsync_ShouldReturnTrue_WhenTokenIsExpired()
        {
            // Arrange
            const string tokenValue = "expiredToken";
            _tokenManagerServiceMock.Setup(tms => tms.ExpiredTokenAsync(tokenValue)).ReturnsAsync(true);

            // Act
            var result = await _authenticateService.ExpiredTokenServiceAsync(tokenValue);

            // Assert
            Assert.True(result);
            _tokenManagerServiceMock.Verify(tms => tms.ExpiredTokenAsync(tokenValue), Times.Once);
        }

        [Fact]
        public async Task ExpiredTokenAsync_ShouldReturnFalse_WhenTokenIsNotExpired()
        {
            // Arrange
            const string tokenValue = "validToken";
            _tokenManagerServiceMock.Setup(tms => tms.ExpiredTokenAsync(tokenValue)).ReturnsAsync(false);

            // Act
            var result = await _authenticateService.ExpiredTokenServiceAsync(tokenValue);

            // Assert
            Assert.False(result);
            _tokenManagerServiceMock.Verify(tms => tms.ExpiredTokenAsync(tokenValue), Times.Once);
        }
    }
}