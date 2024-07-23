﻿using AIIncidentAnalysisAuthServiceAPI.Repositories;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;

namespace AIIncidentAnalysisAuthServiceAPI.Extensions;

public static class DependencyInjectionRepositories
{
    public static void AddDependencyInjectionRepositories(this IServiceCollection service)
    {
        service.AddScoped<IAuthRepository, AuthRepository>();
        service.AddScoped<IUserRoleRepository, UserRoleRepository>();
        service.AddScoped<ITokenRepository, TokenRepository>();
    }
}