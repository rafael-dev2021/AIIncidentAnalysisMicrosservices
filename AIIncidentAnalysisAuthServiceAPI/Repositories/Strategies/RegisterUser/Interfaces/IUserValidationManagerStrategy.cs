namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;

public interface IUserValidationManagerStrategy
{
    Task<List<string>> ValidateUserDetailsAsync(string? cpf, string? email, string? phoneNumber);
}