using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Models;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser.Interfaces;

public interface IUserFieldUpdaterStrategy
{
    void UpdateFields(PoliceOfficer user, UpdateUserDtoRequest updateUserDtoRequest);
}