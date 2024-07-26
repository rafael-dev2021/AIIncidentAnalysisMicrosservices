﻿using System.Security.Claims;
using System.Text.Json;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace AIIncidentAnalysisAuthServiceAPI.Middleware;

public class SecurityFilterMiddleware(RequestDelegate next)
{
    private readonly List<string> _ignorePaths =
    [
        "/api/v1/auth/login",
        "/api/v1/auth/register",
        "/api/v1/auth/forgot-password"
    ];

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        try
        {
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && _ignorePaths.Contains(path))
            {
                await next(context);
                return;
            }

            var tokenService = serviceProvider.GetRequiredService<ITokenService>();
            var tokenRepository = serviceProvider.GetRequiredService<ITokenRepository>();
            var userManager = serviceProvider.GetRequiredService<UserManager<PoliceOfficer>>();

            var token = RecoverTokenFromRequest(context.Request);
            if (token != null)
            {
                var isAuthenticated =
                    await HandleAuthentication(context, token, tokenService, tokenRepository, userManager);
                if (isAuthenticated)
                {
                    await next(context);
                    return;
                }
            }

            var errorResponse = new { Message = "User is not authenticated." };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
        catch (Exception e)
        {
            Log.Error(e, "[ERROR_FILTER] Error processing security filter: [{Message}]", e.Message);
        }
    }

    public string? RecoverTokenFromRequest(HttpRequest request)
    {
        if (request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString().Replace("Bearer ", string.Empty);
            Log.Information("[TOKEN_RECOVERED] Token recovered from Authorization header: [{Token}]", token);
            return token;
        }

        Log.Information("[NO_AUTH_HEADER] No Authorization header found in the request.");
        return null;
    }

    public async Task<bool> HandleAuthentication(HttpContext context, string token, ITokenService tokenService,
        ITokenRepository tokenRepository, UserManager<PoliceOfficer> userManager)
    {
        try
        {
            var email = tokenService.ValidateToken(token)?.Identity?.Name;
            if (email != null)
            {
                var user = await userManager.FindByEmailAsync(email);
                var isTokenValid = await IsTokenValid(tokenRepository, token);

                if (user != null && isTokenValid)
                {
                    SetAuthenticationInSecurityContext(context, user);
                    Log.Information(
                        "[USER_AUTHENTICATED] User: [{UserName}] successfully authenticated with token: [{Token}]",
                        user.UserName, token);
                    return true;
                }
            }

            LogError(context, token);
            return false;
        }
        catch (SecurityTokenException e)
        {
            await HandleInvalidToken(context.Response, e);
            LogError(context, token);
            return false;
        }
    }

    private static void LogError(HttpContext context, string token)
    {
        Log.Error(
            "[TOKEN_FAILED] User-Agent: [{UserAgent}] IP Address: [{IpAddress}]. Validation failed for token: [{Token}]",
            GetUserAgent(context), GetIpAddress(context), token);
    }

    private static string? GetIpAddress(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString();
    }

    private static string GetUserAgent(HttpContext context)
    {
        return context.Request.Headers.UserAgent.ToString();
    }

    private static async Task<bool> IsTokenValid(ITokenRepository tokenRepository, string token)
    {
        var dbToken = await tokenRepository.FindByTokenValue(token);
        return dbToken is { TokenExpired: false, TokenRevoked: false };
    }

    private static void SetAuthenticationInSecurityContext(HttpContext context, PoliceOfficer user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!)
        };

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
        context.User = claimsPrincipal;
    }

    private static async Task HandleInvalidToken(HttpResponse response, SecurityTokenException e)
    {
        response.StatusCode = StatusCodes.Status401Unauthorized;
        await response.WriteAsync(e.Message);
        Log.Warning("[TOKEN_INVALID] Invalid token detected: [{Message}]", e.Message);
    }
}