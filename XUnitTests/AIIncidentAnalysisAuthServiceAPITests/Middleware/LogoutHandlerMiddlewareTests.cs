﻿using AIIncidentAnalysisAuthServiceAPI.Middleware;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Middleware;

public class LogoutHandlerMiddlewareTests
{
    private readonly Mock<ITokenRepository> _tokenRepositoryMock = new();
    private readonly Mock<SignInManager<PoliceOfficer>> _signInManagerMock;
    private readonly Mock<RequestDelegate> _nextMock = new();

    public LogoutHandlerMiddlewareTests()
    {
        Mock<IUserStore<PoliceOfficer>> userStoreMock = new();
        UserManager<PoliceOfficer> userManager = new(
            userStoreMock.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
        );

        _signInManagerMock = new Mock<SignInManager<PoliceOfficer>>(
            userManager,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<PoliceOfficer>>(),
            null!,
            null!,
            null!,
            null!
        );
    }

    [Fact]
    public async Task InvokeAsync_LogoutPath_ValidToken_InvalidatesTokenAndLogs()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            Request =
            {
                Path = "/v1/auth/logout"
            }
        };
        context.Request.Headers.Append("Authorization", "Bearer validToken");

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ITokenRepository)))
            .Returns(_tokenRepositoryMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(SignInManager<PoliceOfficer>)))
            .Returns(_signInManagerMock.Object);

        var validToken = new Token();
        _tokenRepositoryMock.Setup(repo => repo.FindByTokenValue("validToken"))
            .ReturnsAsync(validToken);

        var handler = new LogoutHandlerMiddleware(_nextMock.Object);

        // Act
        await handler.InvokeAsync(context, serviceProviderMock.Object);

        // Assert
        _tokenRepositoryMock.Verify(repo => repo.FindByTokenValue("validToken"), Times.Once);
        _tokenRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
        _signInManagerMock.Verify(manager => manager.SignOutAsync(), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_LogoutPath_InvalidToken_SendsUnauthorizedResponse()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            Request =
            {
                Path = "/v1/auth/logout",
                Headers =
                {
                    Authorization = "Bearer invalidToken"
                }
            }
        };

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ITokenRepository)))
            .Returns(_tokenRepositoryMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(SignInManager<PoliceOfficer>)))
            .Returns(_signInManagerMock.Object);

        _tokenRepositoryMock.Setup(repo => repo.FindByTokenValue("invalidToken"))
            .ReturnsAsync((Token)null!);

        var handler = new LogoutHandlerMiddleware(_nextMock.Object);

        // Act
        await handler.InvokeAsync(context, serviceProviderMock.Object);

        // Assert
        _tokenRepositoryMock.Verify(repo => repo.FindByTokenValue("invalidToken"), Times.Once);
        _tokenRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Never);
        _signInManagerMock.Verify(manager => manager.SignOutAsync(), Times.Never);
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_NonLogoutPath_CallsNextDelegate()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            Request =
            {
                Path = "/some/other/path"
            }
        };

        var handler = new LogoutHandlerMiddleware(_nextMock.Object);

        // Act
        await handler.InvokeAsync(context, Mock.Of<IServiceProvider>());

        // Assert
        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_LogoutPath_NoAuthHeader_SendsUnauthorizedResponse()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            Request =
            {
                Path = "/v1/auth/logout"
            }
        };

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ITokenRepository)))
            .Returns(_tokenRepositoryMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(SignInManager<PoliceOfficer>)))
            .Returns(_signInManagerMock.Object);

        var handler = new LogoutHandlerMiddleware(_nextMock.Object);

        // Act
        await handler.InvokeAsync(context, serviceProviderMock.Object);

        // Assert
        _tokenRepositoryMock.Verify(repo => repo.FindByTokenValue(It.IsAny<string>()), Times.Never);
        _tokenRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Never);
        _signInManagerMock.Verify(manager => manager.SignOutAsync(), Times.Never);
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }
}