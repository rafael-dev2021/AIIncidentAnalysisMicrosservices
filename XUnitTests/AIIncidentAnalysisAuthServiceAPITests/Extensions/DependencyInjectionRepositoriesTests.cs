using AIIncidentAnalysisAuthServiceAPI.Algorithms;
using AIIncidentAnalysisAuthServiceAPI.Algorithms.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Extensions;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Extensions;

public class DependencyInjectionRepositoriesTests
{
    private readonly IServiceProvider _serviceProvider;

    public DependencyInjectionRepositoriesTests()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));

        serviceCollection.AddLogging(); 

        serviceCollection.AddIdentity<PoliceOfficer, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
       
        serviceCollection.AddMemoryCache(); 
        serviceCollection.AddDistributedMemoryCache(); 
        serviceCollection.AddDependencyInjectionRepositories();

        serviceCollection.AddDependencyInjectionAuthStrategies();
        serviceCollection.AddDependencyInjectionUpdateStrategies();
        serviceCollection.AddDependencyInjectionRegisterStrategies();
        
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void Test_IUserRoleRepository_IsRegistered()
    {
        var service = _serviceProvider.GetService<IUserRoleRepository>();
        Assert.NotNull(service);
        Assert.IsType<UserRoleRepository>(service);
    }

    [Fact]
    public void Test_IAuthRepository_IsRegistered()
    {
        var service = _serviceProvider.GetService<IAuthRepository>();
        Assert.NotNull(service);
        Assert.IsType<AuthRepository>(service);
    }

    [Fact]
    public void Test_ITokenRepository_IsRegistered()
    {
        var service = _serviceProvider.GetService<ITokenRepository>();
        Assert.NotNull(service);
        Assert.IsType<TokenRepository>(service);
    }

    [Fact]
    public void Test_IAccountNumberGenerator_IsRegistered()
    {
        var service = _serviceProvider.GetService<IAccountNumberGenerator>();
        Assert.NotNull(service);
        Assert.IsType<AccountNumberGenerator>(service);
    }
}
