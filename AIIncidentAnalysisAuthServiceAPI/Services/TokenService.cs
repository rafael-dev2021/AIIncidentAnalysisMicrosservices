using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Services.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Utils;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace AIIncidentAnalysisAuthServiceAPI.Services;

public class TokenService(JwtSettings jwtSettings) : ITokenService
{
    public async Task<string> GenerateToken(PoliceOfficer policeOfficer, int expirationToken)
    {
        Log.Information("[GENERATE_TOKEN] Generating token for user: [{Email}]",
            policeOfficer.Email);

        var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, policeOfficer.Id),
                new Claim(ClaimTypes.Name, policeOfficer.Email!),
                new Claim(ClaimTypes.GivenName, policeOfficer.Name!),
                new Claim(ClaimTypes.Surname, policeOfficer.LastName!),
                new Claim("Cpf", policeOfficer.Cpf!),
                new Claim("PhoneNumber", policeOfficer.PhoneNumber!),
                new Claim(ClaimTypes.Email, policeOfficer.Email!),
                new Claim(ClaimTypes.Role, policeOfficer.Role!),
            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationToken),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        Log.Information("[GENERATE_TOKEN] Token generated successfully for user: [{Email}]", 
            policeOfficer.Email);
        return await Task.FromResult(tokenString);
    }


    public async Task<string> GenerateAccessTokenAsync(PoliceOfficer policeOfficer)
    {
        Log.Information("[ACCESS_TOKEN] Generating access token for user: [{Email}]",
            policeOfficer.Email);
        return await GenerateToken(policeOfficer, jwtSettings.ExpirationTokenMinutes);
    }

    public async Task<string> GenerateRefreshTokenAsync(PoliceOfficer policeOfficer)
    {
        Log.Information("[REFRESH_TOKEN] Generating refresh token for user: [{Email}]",
            policeOfficer.Email);
        return await GenerateToken(policeOfficer, jwtSettings.RefreshTokenExpirationMinutes);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        Log.Information("[VALID_TOKEN] Validating token");
        var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            Log.Information("[VALID_TOKEN] Token validated successfully");
            return principal;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[VALID_TOKEN] Token validation failed: {Message}", ex.Message);
            return null;
        }
    }
}