using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser.Interfaces;

public interface IUpdateProfileStrategy
{
    Task<UpdateDtoResponse> UpdateProfileAsync(UpdateUserDtoRequest updateUserDtoRequest, string userId);
}