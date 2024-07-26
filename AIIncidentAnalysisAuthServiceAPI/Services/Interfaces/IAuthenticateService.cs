using AIIncidentAnalysisAuthServiceAPI.Dto.Request;
using AIIncidentAnalysisAuthServiceAPI.Dto.Response;

namespace AIIncidentAnalysisAuthServiceAPI.Services.Interfaces;

public interface IAuthenticateService
{
    Task<IEnumerable<UserDtoResponse>> ListAllUsersServiceAsync();
    Task<TokenDtoResponse> LoginServiceAsync(LoginDtoRequest request);
    Task<TokenDtoResponse> RegisterServiceAsync(RegisterDtoRequest request);
    Task<TokenDtoResponse> UpdateUserServiceAsync(UpdateUserDtoRequest request, string userId);
    Task<bool> ChangePasswordServiceAsync(ChangePasswordDtoRequest request, string userId);
    Task LogoutServiceAsync();
    Task<bool> ForgotPasswordServiceAsync(ForgotPasswordDtoRequest forgotPasswordDtoRequest);
    Task<TokenDtoResponse> RefreshTokenServiceAsync(RefreshTokenDtoRequest refreshTokenDtoRequest); 
    Task<bool> RevokedTokenServiceAsync(string token);
    Task<bool> ExpiredTokenServiceAsync(string token);
}