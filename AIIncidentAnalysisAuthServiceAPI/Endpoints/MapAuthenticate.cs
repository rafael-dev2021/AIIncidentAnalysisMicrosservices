﻿using System.Security.Claims;
using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Endpoints.Strategies;
using AIIncidentAnalysisAuthServiceAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AIIncidentAnalysisAuthServiceAPI.Endpoints;

public static class MapAuthenticate
{
    private const string CacheKey = "cached_users";

    public static void MapAuthenticateEndpoints(this WebApplication app)
    {
        MapGetUsersEndpoint<IEnumerable<UserDtoResponse>>(
            app, "api/v1/auth/users",
            async service => await service.ListAllUsersServiceAsync()
        );

        MapPostUnauthorizedEndpoint<LoginDtoRequest>(
            app,
            "/api/v1/auth/login",
            async (service, request) =>
                Results.Ok(await service.LoginServiceAsync(request))
        );
        
        MapPostUnauthorizedEndpoint<RegisterDtoRequest>(
            app,
            "/api/v1/auth/register",
            async (service, request) =>
            {
                var response = await service.RegisterServiceAsync(request);
                return Results.Created($"/api/v1/auth/users/{request.Email}", response);
            }
        );
        
        MapPostUnauthorizedEndpoint<ForgotPasswordDtoRequest>(
            app,
            "/v1/auth/forgot-password",
            async (service, request) =>
            {
                var success = await service.ForgotPasswordServiceAsync(request);
                return Results.Ok(success);
            }
        );
        
        MapPostLogoutEndpoint(
            app,
            "/v1/auth/logout",
            async service =>
            {
                await service.LogoutServiceAsync();
                return null!;
            },
            expectedStatusCode: StatusCodes.Status204NoContent
        );
        
        MapPutAuthorizedEndpoint<UpdateUserDtoRequest>(
            app,
            "/v1/auth/update-profile",
            async (service, request, userId) => await service.UpdateUserServiceAsync(request, userId)
        );
        
        MapPutAuthorizedEndpoint<ChangePasswordDtoRequest>(
            app,
            "/v1/auth/change-password",
            async (service, request, userId) => await service.ChangePasswordServiceAsync(request, userId)
        );
        
        MapPostAuthorizeEndpoint<RefreshTokenDtoRequest>(
            app,
            "/v1/auth/refresh-token",
            async (service, request) =>
            {
                var success = await service.RefreshTokenServiceAsync(request);
                return Results.Ok(success);
            });

        MapGetTokenValidationEndpoint(
            app,
            "/v1/auth/revoked-token",
            async (service, token) =>
            {
                var success = await service.RevokedTokenServiceAsync(token);
                return success;
            },
            "cached_revoked"
        );

        MapGetTokenValidationEndpoint(
            app,
            "/v1/auth/expired-token",
            async (service, token) =>
            {
                var success = await service.ExpiredTokenServiceAsync(token);
                return success;
            },
            "cached_expired"
        );
    }

    private static void MapGetUsersEndpoint<T>(
        WebApplication app,
        string route,
        Func<IAuthenticateService, Task<T>> handler)
    {
        app.MapGet(route, async (
            [FromServices] IAuthenticateService service,
            [FromServices] IDistributedCache cache,
            HttpContext context) =>
        {
            try
            {
                var authResult = AuthenticationRules.CheckReadWriteRole(context);
                if (authResult != null)
                {
                    return authResult;
                }

                var cachedUsers = await cache.GetStringAsync(CacheKey);
                if (!string.IsNullOrEmpty(cachedUsers))
                {
                    var usersDeserializers = JsonConvert.DeserializeObject<IEnumerable<UserDtoResponse>>(cachedUsers);
                    return Results.Ok(usersDeserializers);
                }

                var usersDto = await handler(service);
                var serializedUsers = JsonConvert.SerializeObject(usersDto);

                await cache.SetStringAsync(CacheKey, serializedUsers, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                return Results.Ok(usersDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        }).RequireAuthorization();
    }

    private static void MapPostUnauthorizedEndpoint<T>(
        WebApplication app,
        string route,
        Func<IAuthenticateService, T, Task<object>> handler) where T : class
    {
        app.MapPost(route, async (
            [FromServices] IAuthenticateService service,
            [FromBody] T request,
            [FromServices] IValidator<T> validator,
            [FromServices] IDistributedCache cache) =>
        {
            try
            {
                var validationResult = await RequestValidator.ValidateAsync(request, validator);
                if (validationResult != null)
                {
                    return validationResult;
                }

                await cache.RemoveAsync(CacheKey);

                var result = await handler(service, request);
                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        });
    }
    
    private static void MapPostLogoutEndpoint(
        WebApplication app,
        string route,
        Func<IAuthenticateService, Task<object>> handler,
        int expectedStatusCode = StatusCodes.Status204NoContent)
    {
        app.MapPost(route, async (
            [FromServices] IAuthenticateService service) =>
        {
            try
            {
                await handler(service);
                return Results.StatusCode(expectedStatusCode);
            }
            catch (UnauthorizedAccessException ex)
            {
                var errorResponse = new Dictionary<string, string> { { "Message", ex.Message } };
                return Results.Json(errorResponse, statusCode: StatusCodes.Status400BadRequest);
            }
        }).RequireAuthorization();
    }
    
    private static void MapPutAuthorizedEndpoint<T>(
        WebApplication app,
        string route,
        Func<IAuthenticateService, T, string, Task<object>> handler) where T : class
    {
        app.MapPut(route, async (
            [FromServices] IAuthenticateService service,
            [FromBody] T request,
            HttpContext context,
            [FromServices] IDistributedCache cache,
            [FromServices] IValidator<T> validator) =>
        {
            try
            {
                var validationResult = await RequestValidator.ValidateAsync(request, validator);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                await cache.RemoveAsync(CacheKey);

                return await handler(service, request, userId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        }).RequireAuthorization();
    }
    
    private static void MapPostAuthorizeEndpoint<T>(
        WebApplication app,
        string route,
        Func<IAuthenticateService, T, Task<IResult>> handler) where T : class
    {
        app.MapPost(route, async (
            [FromServices] IAuthenticateService service,
            [FromServices] IDistributedCache cache,
            [FromBody] T request) =>
        {
            try
            {
                await cache.RemoveAsync(CacheKey);

                var result = await handler(service, request);
                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                return AuthenticationRules.HandleUnauthorizedAccessException(ex);
            }
        });
    }
    
    private static void MapGetTokenValidationEndpoint(
        WebApplication app,
        string route,
        Func<IAuthenticateService, string, Task<bool>> handler,
        string cacheKeyPrefix)
    {
        app.MapGet(route, async (
            [FromServices] IAuthenticateService service,
            [FromServices] IDistributedCache cache,
            [FromQuery] string token) =>
        {
            var cacheKey = $"{cacheKeyPrefix}_{token}";

            var cachedResult = await cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedResult))
            {
                var resultCached = JsonConvert.DeserializeObject<ApiTokensDtoResponse>(cachedResult);
                return Results.Ok(resultCached!.Success);
            }

            var success = await handler(service, token);
            var apiTokensDtoResponse = new ApiTokensDtoResponse(success);

            var serializedResult = JsonConvert.SerializeObject(apiTokensDtoResponse);
            await cache.SetStringAsync(cacheKey, serializedResult, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return Results.Ok(success);
        }).RequireAuthorization();
    }
}