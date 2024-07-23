using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.AuthUser.Interfaces;

public interface IAuthStrategy
{
    Task<AuthDtoResponse> AuthenticatedAsync(LoginDtoRequest request);
}