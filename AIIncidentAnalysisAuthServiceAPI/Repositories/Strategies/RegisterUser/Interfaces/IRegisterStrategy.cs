using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;

public interface IRegisterStrategy
{
    Task<RegisterDtoResponse> CreateUserAsync(RegisterDtoRequest request);
}