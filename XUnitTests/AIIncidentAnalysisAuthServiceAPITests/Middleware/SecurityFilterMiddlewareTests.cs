﻿using System.Net;
using System.Security.Claims;
using AIIncidentAnalysisAuthServiceAPI.Middleware;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Serilog;
using Serilog.Sinks.TestCorrelator;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Middleware;

public class SecurityFilterMiddlewareTests
{
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<ITokenRepository> _mockTokenRepository;
    private readonly Mock<UserManager<PoliceOfficer>> _mockUserManager;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly SecurityFilterMiddleware _middleware;

    protected SecurityFilterMiddlewareTests()
    {
        _mockTokenService = new Mock<ITokenService>();
        _mockTokenRepository = new Mock<ITokenRepository>();
        _mockUserManager = new Mock<UserManager<PoliceOfficer>>(
            Mock.Of<IUserStore<PoliceOfficer>>(), null!, null!, null!, null!, null!, null!, null!, null!
        );
        _mockHttpContext = new Mock<HttpContext>();
        Mock<HttpRequest> mockHttpRequest = new();
        Mock<HttpResponse> mockHttpResponse = new();
        _mockNext = new Mock<RequestDelegate>();
        _middleware = new SecurityFilterMiddleware(_mockNext.Object);

        _mockHttpContext.Setup(c => c.Request).Returns(mockHttpRequest.Object);
        _mockHttpContext.Setup(c => c.Response).Returns(mockHttpResponse.Object);

        var mockConnection = new Mock<ConnectionInfo>();
        mockConnection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Parse("127.0.0.1"));
        _mockHttpContext.Setup(c => c.Connection).Returns(mockConnection.Object);

        var headers = new HeaderDictionary
        {
            { "User-Agent", "MockUserAgent" }
        };
        mockHttpRequest.Setup(r => r.Headers).Returns(headers);

        var responseBody = new MemoryStream();
        mockHttpResponse.Setup(r => r.Body).Returns(responseBody);
    }

    public class InvokeAsyncTests : SecurityFilterMiddlewareTests
    {
        [Fact]
        public async Task Should_Ignore_Specified_Paths()
        {
            // Arrange
            var ignoredPaths = new[]
            {
                "/api/v1/auth/login",
                "/api/v1/auth/register",
                "/api/v1/auth/forgot-password"
            };

            foreach (var path in ignoredPaths)
            {
                var context = new DefaultHttpContext();
                context.Request.Path = path;

                var serviceProvider = new ServiceCollection()
                    .AddSingleton(_mockTokenService.Object)
                    .AddSingleton(_mockTokenRepository.Object)
                    .AddSingleton(_mockUserManager.Object)
                    .BuildServiceProvider();

                context.RequestServices = serviceProvider;

                // Act
                await _middleware.InvokeAsync(context, serviceProvider);

                // Assert
                _mockNext.Verify(next => next(context), Times.Once);
            }
        }

        [Fact]
        public async Task InvokeAsync_Should_Proceed_When_Token_Is_Valid()
        {
            // Arrange
            var context = new DefaultHttpContext
            {
                Request =
                {
                    Path = "/some/path"
                }
            };
            const string validToken = "validToken";
            context.Request.Headers.Authorization = $"Bearer {validToken}";

            var token = new Token();
            token.SetTokenValue(validToken);
            token.SetTokenRevoked(false);
            token.SetTokenExpired(false);

            var user = new PoliceOfficer { UserName = "user" };
            var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.UserName) });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            _mockTokenService.Setup(s => s.ValidateToken(validToken)).Returns(claimsPrincipal);
            _mockUserManager.Setup(um => um.FindByEmailAsync(user.UserName)).ReturnsAsync(user);
            _mockTokenRepository.Setup(tr => tr.FindByTokenValue(validToken))
                .ReturnsAsync(token);

            var serviceProvider = new ServiceCollection()
                .AddSingleton(_mockTokenService.Object)
                .AddSingleton(_mockTokenRepository.Object)
                .AddSingleton(_mockUserManager.Object)
                .BuildServiceProvider();

            context.RequestServices = serviceProvider;

            // Act
            await _middleware.InvokeAsync(context, serviceProvider);

            // Assert
            _mockNext.Verify(next => next(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_Should_Return_401_When_Token_Is_Invalid()
        {
            // Arrange
            var context = new DefaultHttpContext
            {
                Request =
                {
                    Path = "/some/path"
                }
            };
            const string token = "invalidToken";
            context.Request.Headers.Authorization = $"Bearer {token}";

            _mockTokenService.Setup(s => s.ValidateToken(token)).Returns<ClaimsPrincipal>(null!);

            var serviceProvider = new ServiceCollection()
                .AddSingleton(_mockTokenService.Object)
                .AddSingleton(_mockTokenRepository.Object)
                .AddSingleton(_mockUserManager.Object)
                .BuildServiceProvider();

            context.RequestServices = serviceProvider;

            // Act
            await _middleware.InvokeAsync(context, serviceProvider);

            // Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
            _mockNext.Verify(next => next(context), Times.Never);
        }

        [Fact]
        public async Task Should_Log_Error_On_Invocation_Failure()
        {
            // Arrange
            const string exceptionMessage = "Test exception";
            var exception = new Exception(exceptionMessage);

            var filter = new SecurityFilterMiddleware(_mockNext.Object);

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(ITokenService)))
                .Throws(exception);

            _mockHttpContext.Setup(c => c.RequestServices).Returns(mockServiceProvider.Object);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                // Act
                await filter.InvokeAsync(_mockHttpContext.Object, mockServiceProvider.Object);

                // Assert
                var logEvents = TestCorrelator.GetLogEventsFromCurrentContext();
                Assert.Contains(logEvents,
                    logEvent => logEvent.MessageTemplate.Text.Contains(
                        "[ERROR_FILTER] Error processing security filter"));
                Assert.Contains(logEvents, logEvent => logEvent.Exception?.Message == exceptionMessage);
            }
        }
    }

    public class RecoverTokenFromRequestTests : SecurityFilterMiddlewareTests
    {
        [Fact]
        public void Should_Return_Token_When_Authorization_Header_Present()
        {
            // Arrange
            var request = new Mock<HttpRequest>();
            var headers = new HeaderDictionary
            {
                { "Authorization", "Bearer validToken" }
            };
            request.Setup(r => r.Headers).Returns(headers);

            var securityFilter = new SecurityFilterMiddleware(_mockNext.Object);

            // Act
            var token = securityFilter.RecoverTokenFromRequest(request.Object);

            // Assert
            Assert.NotNull(token);
            Assert.Equal("validToken", token);
        }

        [Fact]
        public void Should_Return_Null_When_Authorization_Header_Not_Present()
        {
            // Arrange
            var request = new Mock<HttpRequest>();
            var headers = new HeaderDictionary();
            request.Setup(r => r.Headers).Returns(headers);

            var securityFilter = new SecurityFilterMiddleware(_mockNext.Object);

            // Act
            var token = securityFilter.RecoverTokenFromRequest(request.Object);

            // Assert
            Assert.Null(token);
        }

        [Fact]
        public void Should_Log_Token_Recovered_When_Authorization_Header_Present()
        {
            // Arrange
            var request = new Mock<HttpRequest>();
            var headers = new HeaderDictionary
            {
                { "Authorization", "Bearer validToken" }
            };
            request.Setup(r => r.Headers).Returns(headers);

            var securityFilter = new SecurityFilterMiddleware(_mockNext.Object);

            // Act
            var token = securityFilter.RecoverTokenFromRequest(request.Object);

            // Assert
            Assert.NotNull(token);
            Assert.Equal("validToken", token);
        }

        [Fact]
        public void Should_Log_No_Auth_Header_When_Authorization_Header_Not_Present()
        {
            // Arrange
            var request = new Mock<HttpRequest>();
            var headers = new HeaderDictionary();
            request.Setup(r => r.Headers).Returns(headers);

            var securityFilter = new SecurityFilterMiddleware(_mockNext.Object);

            // Act
            var token = securityFilter.RecoverTokenFromRequest(request.Object);

            // Assert
            Assert.Null(token);
        }
    }

    public class HandleAuthenticationTests : SecurityFilterMiddlewareTests
    {
        [Fact]
        public async Task HandleAuthentication_UserNotFound_ShouldLogErrorAndReturnFalse()
        {
            // Arrange
            const string token = "invalidToken";
            const string email = "user@example.com";

            _mockTokenService.Setup(ts => ts.ValidateToken(token))
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) })));
            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync((PoliceOfficer)null!);

            var filter = new SecurityFilterMiddleware(_mockNext.Object);

            // Act
            var result = await filter.HandleAuthentication(_mockHttpContext.Object, token, _mockTokenService.Object,
                _mockTokenRepository.Object, _mockUserManager.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task HandleAuthentication_ValidTokenAndUserFound_ShouldReturnTrue()
        {
            // Arrange
            const string token = "validToken";
            const string email = "user@example.com";
            var user = new PoliceOfficer { UserName = "user", Email = email };

            var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var validToken = new Token();
            validToken.SetTokenValue(token);
            validToken.SetTokenExpired(false);
            validToken.SetTokenRevoked(false);

            _mockTokenService.Setup(ts => ts.ValidateToken(token)).Returns(claimsPrincipal);
            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(user);
            _mockTokenRepository.Setup(tr => tr.FindByTokenValue(token)).ReturnsAsync(validToken);

            var filter = new SecurityFilterMiddleware(_mockNext.Object);

            // Act
            var result = await filter.HandleAuthentication(_mockHttpContext.Object, token, _mockTokenService.Object,
                _mockTokenRepository.Object, _mockUserManager.Object);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task HandleAuthentication_Should_Return_False_For_Invalid_Token_Format()
        {
            // Arrange
            var context = new DefaultHttpContext();
            const string token = "invalid_format_token";

            _mockTokenService.Setup(s => s.ValidateToken(token))
                .Throws<SecurityTokenException>();

            // Act
            var result = await _middleware.HandleAuthentication(context, token, _mockTokenService.Object,
                _mockTokenRepository.Object, _mockUserManager.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task HandleAuthentication_Should_Return_False_For_Expired_Token()
        {
            // Arrange
            var context = new DefaultHttpContext();
            const string token = "expired_token";
            const string email = "user@example.com";
            var user = new PoliceOfficer { UserName = "user", Email = email };

            var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var expiredToken = new Token();
            expiredToken.SetTokenValue(token);
            expiredToken.SetTokenExpired(true);
            expiredToken.SetTokenRevoked(false);

            _mockTokenService.Setup(ts => ts.ValidateToken(token)).Returns(claimsPrincipal);
            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(user);
            _mockTokenRepository.Setup(tr => tr.FindByTokenValue(token)).ReturnsAsync(expiredToken);

            // Act
            var result = await _middleware.HandleAuthentication(context, token, _mockTokenService.Object,
                _mockTokenRepository.Object, _mockUserManager.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task HandleAuthentication_Should_Return_False_For_Revoked_Token()
        {
            // Arrange
            var context = new DefaultHttpContext();
            const string token = "revoked_token";
            const string email = "user@example.com";
            var user = new PoliceOfficer { UserName = "user", Email = email };

            var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var revokedToken = new Token();
            revokedToken.SetTokenValue(token);
            revokedToken.SetTokenExpired(false);
            revokedToken.SetTokenRevoked(true);

            _mockTokenService.Setup(ts => ts.ValidateToken(token)).Returns(claimsPrincipal);
            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(user);
            _mockTokenRepository.Setup(tr => tr.FindByTokenValue(token)).ReturnsAsync(revokedToken);

            // Act
            var result = await _middleware.HandleAuthentication(context, token, _mockTokenService.Object,
                _mockTokenRepository.Object, _mockUserManager.Object);

            // Assert
            Assert.False(result);
        }
    }
}