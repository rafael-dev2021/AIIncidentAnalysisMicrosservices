using System.Security.Claims;
using AIIncidentAnalysisAuthServiceAPI.Models;

namespace AIIncidentAnalysisAuthServiceAPI.Services.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(PoliceOfficer policeOfficer);
    Task<string> GenerateRefreshTokenAsync(PoliceOfficer policeOfficer);
    ClaimsPrincipal? ValidateToken(string token);
}