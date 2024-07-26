using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Models;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser.Interfaces;

public interface IUserDetailsValidatorStrategy
{
    Task<List<string>> ValidateAsync(UpdateUserDtoRequest updateUserDtoRequest, PoliceOfficer user);
    void LogValidationErrors(string userId, List<string> validationErrors);
}