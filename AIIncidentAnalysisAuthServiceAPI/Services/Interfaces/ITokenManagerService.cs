using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Models;

namespace AIIncidentAnalysisAuthServiceAPI.Services.Interfaces;

public interface ITokenManagerService
{
    Task<TokenDtoResponse> GenerateTokenResponseAsync(PoliceOfficer policeOfficer);
    void RevokeAllUserTokens(PoliceOfficer policeOfficer);
    Task<bool> RevokedTokenAsync(string token);
    Task<bool> ExpiredTokenAsync(string token);
}