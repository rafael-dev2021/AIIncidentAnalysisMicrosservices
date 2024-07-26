using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Models;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<IEnumerable<PoliceOfficer>> ListPoliceOfficersAsync();
    Task<AuthDtoResponse> AuthenticateAsync(LoginDtoRequest loginDtoRequest);
    Task<RegisterDtoResponse> RegisterAsync(RegisterDtoRequest registerDtoRequest);
    Task<UpdateDtoResponse> UpdateAsync(UpdateUserDtoRequest updateUserDtoRequest, string userId);
    Task<PoliceOfficer?> GetUserProfileAsync(string? userEmail);
    Task<PoliceOfficer?> GetUserIdProfileAsync(string? userId);
    Task<bool> ChangePasswordAsync(ChangePasswordDtoRequest changePasswordDtoRequest);
    Task<bool> ForgotPasswordAsync(ForgotPasswordDtoRequest forgotPasswordDtoRequest);
    Task SaveUserAsync(PoliceOfficer policeOfficer);
    Task LogoutAsync();
}