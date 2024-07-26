using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Endpoints;
using AIIncidentAnalysisAuthServiceAPI.Endpoints.Strategies;
using AIIncidentAnalysisAuthServiceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Endpoints;

public class MapAuthenticateTests
{
    private readonly Mock<IAuthenticateService> _authenticateServiceMock = new();

    public class GetAllUsersTests : MapAuthenticateTests
    {
        [Fact]
        public async Task GetAllUsers_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var usersDtoResponse = new List<UserDtoResponse>
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

            _authenticateServiceMock.Setup(s => s.ListAllUsersServiceAsync()).ReturnsAsync(usersDtoResponse);

            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();
            app.MapAuthenticateEndpoints();

            // Act
            var result = await AuthenticateEndpointTestsHelper.InvokeGetEndpoint(
                _authenticateServiceMock.Object,
                async service => await service.ListAllUsersServiceAsync()
            );

            // Assert
            var okResult = Assert.IsType<Ok<IEnumerable<UserDtoResponse>>>(result);
            Assert.Equal(usersDtoResponse, okResult.Value);
        }
    }

    private static class AuthenticateEndpointTestsHelper
    {
        public static async Task<IResult> InvokeGetEndpoint<T>(
            IAuthenticateService service,
            Func<IAuthenticateService, Task<T>> handler)
        {
            try
            {
                var response = await handler(service);
                return Results.Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        }
    }
}