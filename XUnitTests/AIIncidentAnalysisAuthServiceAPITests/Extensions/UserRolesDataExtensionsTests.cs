using AIIncidentAnalysisAuthServiceAPI.Extensions;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Extensions;

public class UserRolesDataExtensionsTests
{
    [Fact]
    public async Task AddUserRolesDataExtensions_ShouldCallRoleAndUserMethods_WhenRepositoryIsNotNull()
    {
        // Arrange
        var userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(),
            null!, null!, null!, null!
        );

        var serviceProviderMock = new Mock<IServiceProvider>();
        var serviceScopeMock = new Mock<IServiceScope>();
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var applicationBuilderMock = new Mock<IApplicationBuilder>();

        serviceProviderMock.Setup(sp => sp.GetService(typeof(IUserRoleRepository)))
            .Returns(userRoleRepositoryMock.Object);

        serviceProviderMock.Setup(sp => sp.GetService(typeof(RoleManager<IdentityRole>)))
            .Returns(roleManagerMock.Object);

        serviceScopeMock.Setup(s => s.ServiceProvider)
            .Returns(serviceProviderMock.Object);

        serviceScopeFactoryMock.Setup(sf => sf.CreateScope())
            .Returns(serviceScopeMock.Object);

        applicationBuilderMock.Setup(ab => ab.ApplicationServices.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactoryMock.Object);

        // Act
        await UserRolesDataExtensions.AddUserRolesDataExtensions(applicationBuilderMock.Object);

        // Assert
        userRoleRepositoryMock.Verify(r => r.RoleAsync(), Times.Once);
        userRoleRepositoryMock.Verify(r => r.UserAsync(), Times.Once);
    }

    [Fact]
    public async Task AddUserRolesDataExtensions_ShouldNotThrow_WhenRepositoryIsNull()
    {
        // Arrange
        var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(),
            null!, null!, null!, null!
        );

        var serviceProviderMock = new Mock<IServiceProvider>();
        var serviceScopeMock = new Mock<IServiceScope>();
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var applicationBuilderMock = new Mock<IApplicationBuilder>();

        serviceProviderMock.Setup(sp => sp.GetService(typeof(IUserRoleRepository)))
            .Returns(null!);

        serviceProviderMock.Setup(sp => sp.GetService(typeof(RoleManager<IdentityRole>)))
            .Returns(roleManagerMock.Object);

        serviceScopeMock.Setup(s => s.ServiceProvider)
            .Returns(serviceProviderMock.Object);

        serviceScopeFactoryMock.Setup(sf => sf.CreateScope())
            .Returns(serviceScopeMock.Object);

        applicationBuilderMock.Setup(ab => ab.ApplicationServices.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactoryMock.Object);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            UserRolesDataExtensions.AddUserRolesDataExtensions(applicationBuilderMock.Object));

        // Assert
        Assert.Null(exception);
    }
}