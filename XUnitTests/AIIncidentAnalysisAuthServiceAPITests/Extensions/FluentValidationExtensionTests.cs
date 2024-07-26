using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Extensions;
using AIIncidentAnalysisAuthServiceAPI.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Extensions;

public class FluentValidationExtensionTests
{
    [Fact]
    public void AddFluentValidationDependencyInjection_ShouldRegisterValidators()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMvc();

        // Act
        serviceCollection.AddFluentValidationExtension();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IValidator<ChangePasswordDtoRequest>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<LoginDtoRequest>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<UpdateUserDtoRequest>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<AuthDtoResponse>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<RegisterDtoResponse>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<PoliceOfficer>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<RegisterDtoRequest>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<UpdateDtoResponse>>());
        Assert.NotNull(serviceProvider.GetService<IValidator<TokenDtoResponse>>());
    }
}