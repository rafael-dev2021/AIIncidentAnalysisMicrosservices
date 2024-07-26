using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using AIIncidentAnalysisAuthServiceAPI.Repositories;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories;

public class AuthRepositoryTests
{
    private readonly Mock<IAuthStrategy> _authenticatedStrategyMock;
    private readonly AuthRepository _authenticatedRepository;
    private readonly Mock<IRegisterStrategy> _registerStrategyMock;
    private readonly Mock<IUpdateProfileStrategy> _updateProfileStrategyMock;
    private readonly Mock<UserManager<PoliceOfficer>> _userManagerMock;
    private readonly Mock<SignInManager<PoliceOfficer>> _signInManagerMock;
    private readonly AppDbContext _appDbContext;

    protected AuthRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);

        _registerStrategyMock = new Mock<IRegisterStrategy>();
        _authenticatedStrategyMock = new Mock<IAuthStrategy>();
        _updateProfileStrategyMock = new Mock<IUpdateProfileStrategy>();
        _signInManagerMock = new Mock<SignInManager<PoliceOfficer>>();

        var userStoreMock = new Mock<IUserStore<PoliceOfficer>>();
        _userManagerMock =
            new Mock<UserManager<PoliceOfficer>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<PoliceOfficer>>();
        _signInManagerMock = new Mock<SignInManager<PoliceOfficer>>(
            _userManagerMock.Object,
            contextAccessorMock.Object,
            userPrincipalFactoryMock.Object,
            null!, null!, null!, null!
        );

        _authenticatedRepository = new AuthRepository(
            _signInManagerMock.Object,
            _userManagerMock.Object,
            _authenticatedStrategyMock.Object,
            _registerStrategyMock.Object,
            _updateProfileStrategyMock.Object,
            _appDbContext
        );
    }

    public class GetAllUsersAsyncTests : AuthRepositoryTests
    {
        [Fact(DisplayName = "GetAllUsersAsync should return users with roles")]
        public async Task GetAllUsersAsync_Should_Return_Users_With_Roles()
        {
            // Arrange
            var user1 = new PoliceOfficer { Id = "1", Email = "user1@example.com", PhoneNumber = "+5540028922" };
            user1.SetIdentificationNumber("RG12345");
            user1.SetName("John Doe");
            user1.SetLastName("John Doe");
            user1.SetCpf("123.542.123-20");
            user1.SetBadgeNumber("RG12346");
            user1.SetRole("Admin");
            user1.SetDateOfBirth(DateTime.Now.AddYears(25));
            user1.SetDateOfJoining(DateTime.Now.AddYears(-5));
            user1.SetERank(ERank.Captain);
            user1.SetEDepartment(EDepartment.Administrative);
            user1.SetEOfficerStatus(EOfficerStatus.Active);
            user1.SetEAccessLevel(EAccessLevel.Admin);


            var user2 = new PoliceOfficer { Id = "2", Email = "user2@example.com", PhoneNumber = "+5540028921" };
            user2.SetIdentificationNumber("RG2345");
            user2.SetName("John Doe");
            user2.SetLastName("John Doe");
            user2.SetCpf("123.542.123-21");
            user2.SetBadgeNumber("RG1346");
            user2.SetRole("ReadOnly");
            user2.SetDateOfBirth(DateTime.Now.AddYears(25));
            user2.SetDateOfJoining(DateTime.Now.AddYears(-5));
            user2.SetERank(ERank.Constable);
            user2.SetEDepartment(EDepartment.TrafficDivision);
            user2.SetEOfficerStatus(EOfficerStatus.Active);
            user2.SetEAccessLevel(EAccessLevel.ReadOnly);

            _appDbContext.Users.AddRange(user1, user2);
            await _appDbContext.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<PoliceOfficer>())).ReturnsAsync(new List<string> { "Admin" });

            // Act
            var result = await _authenticatedRepository.ListPoliceOfficersAsync();

            // Assert
            var enumerable = result as PoliceOfficer[] ?? result.ToArray();
            enumerable.Should().HaveCount(2);
            enumerable.First().Role.Should().Be("Admin");
            _userManagerMock.Verify(um => um.GetRolesAsync(It.IsAny<PoliceOfficer>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "GetAllUsersAsync should handle empty user list")]
        public async Task GetAllUsersAsync_Should_Handle_Empty_User_List()
        {
            // Act
            var result = await _authenticatedRepository.ListPoliceOfficersAsync();

            // Assert
            result.Should().BeEmpty();
            _userManagerMock.Verify(um => um.GetRolesAsync(It.IsAny<PoliceOfficer>()), Times.Never);
        }
    }

    public class AuthenticateAsyncTests : AuthRepositoryTests
    {
        [Fact(DisplayName = "AuthenticateAsync should return success response when login is successful")]
        public async Task AuthenticateAsync_Should_Return_Success_Response()
        {
            // Arrange
            var request = new LoginDtoRequest("john@example.com", "password123", true);
            var expectedResponse = new AuthDtoResponse(true, "Login successful.");

            _authenticatedStrategyMock
                .Setup(s => s.AuthenticatedAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.AuthenticateAsync(request);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _authenticatedStrategyMock.Verify(s => s.AuthenticatedAsync(request), Times.Once);
        }

        [Fact(DisplayName = "AuthenticateAsync should return error response when login fails")]
        public async Task AuthenticateAsync_Should_Return_Error_Response()
        {
            // Arrange
            var request = new LoginDtoRequest("john@example.com", "wrongpassword", true);
            var expectedResponse = new AuthDtoResponse(false, "Invalid email or password. Please try again.");

            _authenticatedStrategyMock
                .Setup(s => s.AuthenticatedAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.AuthenticateAsync(request);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _authenticatedStrategyMock.Verify(s => s.AuthenticatedAsync(request), Times.Once);
        }
    }

    public class RegisterAsyncTests : AuthRepositoryTests
    {
        [Fact(DisplayName = "RegisterAsync should return success response when registration is successful")]
        public async Task RegisterAsync_Should_Return_Success_Response()
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

            var expectedResponse = new RegisterDtoResponse(true, "Registration successful.");

            _registerStrategyMock
                .Setup(s => s.CreateUserAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.RegisterAsync(request);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _registerStrategyMock.Verify(s => s.CreateUserAsync(request), Times.Once);
        }
    }

    public class UpdateProfileAsyncTests : AuthRepositoryTests
    {
        [Fact(DisplayName = "UpdateProfileAsync should return success response when profile update is successful")]
        public async Task UpdateProfileAsync_Should_Return_Success_Response()
        {
            // Arrange
            const string userId = "12345";
            var request = new UpdateUserDtoRequest("NewName", "NewLastName", "new.email@example.com", "+1234567890");
            var expectedResponse = new UpdateDtoResponse(true, "Profile updated successfully.");

            _updateProfileStrategyMock
                .Setup(s => s.UpdateProfileAsync(request, userId))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.UpdateAsync(request, userId);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _updateProfileStrategyMock.Verify(s => s.UpdateProfileAsync(request, userId), Times.Once);
        }

        [Fact(DisplayName = "UpdateProfileAsync should return error response when profile update fails")]
        public async Task UpdateProfileAsync_Should_Return_Error_Response()
        {
            // Arrange
            const string userId = "12345";
            var request = new UpdateUserDtoRequest("NewName", "NewLastName", "new.email@example.com", "+1234567890");
            var validationErrors = new List<string> { "Email already used by another user." };
            var expectedResponse = new UpdateDtoResponse(false, string.Join(Environment.NewLine, validationErrors));

            _updateProfileStrategyMock
                .Setup(s => s.UpdateProfileAsync(request, userId))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _authenticatedRepository.UpdateAsync(request, userId);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            _updateProfileStrategyMock.Verify(s => s.UpdateProfileAsync(request, userId), Times.Once);
        }
    }

    public class ChangePasswordAsyncTests : AuthRepositoryTests
    {
        [Fact(DisplayName = "ChangePasswordAsync should return true when password change is successful")]
        public async Task ChangePasswordAsync_Should_Return_True_When_Password_Change_Is_Successful()
        {
            // Arrange
            var request = new ChangePasswordDtoRequest(
                "john@example.com",
                "OldP@ssword",
                "NewP@ssword123");

            var user = new PoliceOfficer { Email = "john@example.com" };

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(request.Email!))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(um => um.ChangePasswordAsync(user, request.CurrentPassword!, request.NewPassword!))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authenticatedRepository.ChangePasswordAsync(request);

            // Assert
            result.Should().BeTrue();
            _userManagerMock.Verify(um => um.FindByEmailAsync(request.Email!), Times.Once);
            _userManagerMock.Verify(um => um.ChangePasswordAsync(user, request.CurrentPassword!, request.NewPassword!),
                Times.Once);
        }

        [Fact(DisplayName = "ChangePasswordAsync should return false when user is not found")]
        public async Task ChangePasswordAsync_Should_Return_False_When_User_Not_Found()
        {
            // Arrange
            var request = new ChangePasswordDtoRequest(
                "nonexistent@example.com",
                "OldP@ssword",
                "NewP@ssword123");

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(request.Email!))
                .ReturnsAsync((PoliceOfficer)null!);

            // Act
            var result = await _authenticatedRepository.ChangePasswordAsync(request);

            // Assert
            result.Should().BeFalse();
            _userManagerMock.Verify(um => um.FindByEmailAsync(request.Email!), Times.Once);
            _userManagerMock.Verify(
                um => um.ChangePasswordAsync(It.IsAny<PoliceOfficer>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact(DisplayName = "ChangePasswordAsync should return false when password change fails")]
        public async Task ChangePasswordAsync_Should_Return_False_When_Password_Change_Fails()
        {
            // Arrange
            var request = new ChangePasswordDtoRequest(
                "john@example.com",
                "OldP@ssword",
                "NewP@ssword123");
            var user = new PoliceOfficer { Email = "john@example.com" };

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(request.Email!))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(um => um.ChangePasswordAsync(user, request.CurrentPassword!, request.NewPassword!))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password change failed." }));

            // Act
            var result = await _authenticatedRepository.ChangePasswordAsync(request);

            // Assert
            result.Should().BeFalse();
            _userManagerMock.Verify(um => um.FindByEmailAsync(request.Email!), Times.Once);
            _userManagerMock.Verify(um => um.ChangePasswordAsync(user, request.CurrentPassword!, request.NewPassword!),
                Times.Once);
        }
    }

    public class GetUserProfileAsyncTests : AuthRepositoryTests
    {
        [Fact(DisplayName = "GetUserProfileAsync should return user profile when user is found")]
        public async Task GetUserProfileAsync_Should_Return_User_Profile_When_User_Is_Found()
        {
            // Arrange
            const string userEmail = "john@example.com";
            var user = new PoliceOfficer
            {
                Email = userEmail,
                PhoneNumber = "+1234567890"
            };
            user.SetName("John");
            user.SetLastName("Doe");

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(userEmail))
                .ReturnsAsync(user);

            // Act
            var result = await _authenticatedRepository.GetUserProfileAsync(userEmail);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be(user.Email);
            result.Name.Should().Be(user.Name);
            result.LastName.Should().Be(user.LastName);
            result.PhoneNumber.Should().Be(user.PhoneNumber);
            _userManagerMock.Verify(um => um.FindByEmailAsync(userEmail), Times.Once);
        }

        [Fact(DisplayName = "GetUserProfileAsync should return null when user is not found")]
        public async Task GetUserProfileAsync_Should_Return_Null_When_User_Is_Not_Found()
        {
            // Arrange
            const string userEmail = "nonexistent@example.com";

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(userEmail))
                .ReturnsAsync((PoliceOfficer)null!);

            // Act
            var result = await _authenticatedRepository.GetUserProfileAsync(userEmail);

            // Assert
            result.Should().BeNull();
            _userManagerMock.Verify(um => um.FindByEmailAsync(userEmail), Times.Once);
        }
    }

    public class GetUserIdProfileAsyncTests : AuthRepositoryTests
    {
        [Fact(DisplayName = "GetUserProfileAsync should return user profile when user is found")]
        public async Task GetUserIdProfileAsync_Should_Return_User_Profile_When_User_Is_Found()
        {
            // Arrange
            const string userEmail = "john@example.com";
            var user = new PoliceOfficer
            {
                Id = "123",
                Email = userEmail,
                PhoneNumber = "+1234567890"
            };
            user.SetName("John");
            user.SetLastName("Doe");

            _userManagerMock
                .Setup(um => um.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            // Act
            var result = await _authenticatedRepository.GetUserIdProfileAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be(user.Email);
            result.Name.Should().Be(user.Name);
            result.LastName.Should().Be(user.LastName);
            result.PhoneNumber.Should().Be(user.PhoneNumber);
            _userManagerMock.Verify(um => um.FindByIdAsync(user.Id), Times.Once);
        }

        [Fact(DisplayName = "GetUserProfileAsync should return null when user is not found")]
        public async Task GetUserIdProfileAsync_Should_Return_Null_When_User_Is_Not_Found()
        {
            // Arrange
            const string userId = "nonexistid";

            _userManagerMock
                .Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((PoliceOfficer)null!);

            // Act
            var result = await _authenticatedRepository.GetUserIdProfileAsync(userId);

            // Assert
            result.Should().BeNull();
            _userManagerMock.Verify(um => um.FindByIdAsync(userId), Times.Once);
        }
    }

    public class ForgotPasswordAsyncTests : AuthRepositoryTests
    {
        [Fact(DisplayName = "ForgotPasswordAsync should return true when password reset is successful")]
        public async Task ForgotPasswordAsync_Should_Return_True_When_Password_Reset_Is_Successful()
        {
            // Arrange
            var request = new ForgotPasswordDtoRequest("john@example.com","NewP@ssword123");

            var user = new PoliceOfficer { Email = request.Email };

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(request.Email!))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(um => um.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("token");

            _userManagerMock
                .Setup(um => um.ResetPasswordAsync(user, "token", request.NewPassword!))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authenticatedRepository.ForgotPasswordAsync(request);

            // Assert
            result.Should().BeTrue();
            _userManagerMock.Verify(um => um.FindByEmailAsync(request.Email!), Times.Once);
            _userManagerMock.Verify(um => um.GeneratePasswordResetTokenAsync(user), Times.Once);
            _userManagerMock.Verify(um => um.ResetPasswordAsync(user, "token", request.NewPassword!), Times.Once);
        }

        [Fact(DisplayName = "ForgotPasswordAsync should return false when user is not found")]
        public async Task ForgotPasswordAsync_Should_Return_False_When_User_Not_Found()
        {
            // Arrange
            var request = new ForgotPasswordDtoRequest("nonexistent@example.com","NewP@ssword123");

            _userManagerMock
                .Setup(um => um.FindByEmailAsync(request.Email!))
                .ReturnsAsync((PoliceOfficer)null!);

            // Act
            var result = await _authenticatedRepository.ForgotPasswordAsync(request);

            // Assert
            result.Should().BeFalse();
            _userManagerMock.Verify(um => um.FindByEmailAsync(request.Email!), Times.Once);
            _userManagerMock.Verify(
                um => um.GeneratePasswordResetTokenAsync(It.IsAny<PoliceOfficer>()),
                Times.Never);
            _userManagerMock.Verify(
                um => um.ResetPasswordAsync(It.IsAny<PoliceOfficer>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }
    }

    public class LogoutAsyncTests : AuthRepositoryTests
    {
        [Fact(DisplayName = "LogoutAsync should sign out successfully")]
        public async Task LogoutAsync_Should_Sign_Out_Successfully()
        {
            // Act
            await _authenticatedRepository.LogoutAsync();

            // Assert
            _signInManagerMock.Verify(sm => sm.SignOutAsync(), Times.Once);
        }
    }

    public class SaveAsyncTests : AuthRepositoryTests
    {
        [Fact(DisplayName = "SaveAsync should update user in the database")]
        public async Task SaveAsync_Should_Update_User_In_Database()
        {
            // Arrange
            var user = new PoliceOfficer { Id = "1", Email = "user1@example.com", PhoneNumber = "+5540028922" };
            user.SetIdentificationNumber("RG12345");
            user.SetName("John Doe");
            user.SetLastName("John Doe");
            user.SetCpf("123.542.123-20");
            user.SetBadgeNumber("RG12346");
            user.SetRole("Admin");
            user.SetDateOfBirth(DateTime.Now.AddYears(25));
            user.SetDateOfJoining(DateTime.Now.AddYears(-5));
            user.SetERank(ERank.Captain);
            user.SetEDepartment(EDepartment.Administrative);
            user.SetEOfficerStatus(EOfficerStatus.Active);
            user.SetEAccessLevel(EAccessLevel.Admin);

            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            // Update user details
            user.SetName("Updated Name 1");
            user.SetLastName("Updated Last Name 1");

            // Act
            await _authenticatedRepository.SaveUserAsync(user);

            // Assert
            var updatedUser = await _appDbContext.Users.FindAsync(user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser!.Name.Should().Be("Updated Name 1");
            updatedUser.LastName.Should().Be("Updated Last Name 1");
        }

        [Fact(DisplayName = "SaveAsync should throw an exception if user is null")]
        public async Task SaveAsync_Should_Throw_Exception_If_User_Is_Null()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _authenticatedRepository.SaveUserAsync(null!));
        }
    }
}