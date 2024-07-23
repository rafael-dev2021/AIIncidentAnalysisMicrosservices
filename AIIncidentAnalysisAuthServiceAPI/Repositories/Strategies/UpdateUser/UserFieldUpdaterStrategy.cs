using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser.Interfaces;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.UpdateUser;

public class UserFieldUpdaterStrategy : IUserFieldUpdaterStrategy
{
    public void UpdateFields(PoliceOfficer user, UpdateUserDtoRequest updateUserDtoRequest)
    {
        UpdateField(user, (u, v) => u.Email = v, user.Email, updateUserDtoRequest.Email);
        UpdateField(user, (u, v) => u.PhoneNumber = v, user.PhoneNumber, updateUserDtoRequest.PhoneNumber);
        UpdateField(user, (u, v) => u.SetName(v), user.Name, updateUserDtoRequest.Name);
        UpdateField(user, (u, v) => u.SetLastName(v), user.LastName, updateUserDtoRequest.LastName);
    }

    private static void UpdateField<T>(PoliceOfficer user, Action<PoliceOfficer, T> setter, T currentValue, T newValue)
    {
        if (!EqualityComparer<T>.Default.Equals(newValue, default) &&
            !EqualityComparer<T>.Default.Equals(newValue, currentValue))
        {
            setter(user, newValue);
        }
    }
}