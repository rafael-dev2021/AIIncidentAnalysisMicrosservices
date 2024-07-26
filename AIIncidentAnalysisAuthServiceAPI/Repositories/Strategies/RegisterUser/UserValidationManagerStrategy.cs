using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Strategies.RegisterUser;

public class UserValidationManagerStrategy(
    ILocalCacheManagerStrategy localCacheManagerStrategy,
    AppDbContext appDbContext)
    : IUserValidationManagerStrategy
{
    public async Task<List<string>> ValidateUserDetailsAsync(string? cpf, string? email, string? phoneNumber)
    {
        var validationErrors = new List<string>();

        if (await IsCpfAlreadyUsedAsync(cpf!)) validationErrors.Add("[CPF already used]");
        if (await IsEmailAlreadyUsedAsync(email!)) validationErrors.Add("[Email already used]");
        if (await IsPhoneNumberAlreadyUsedAsync(phoneNumber!)) validationErrors.Add("[Phone number already used]");

        return validationErrors;
    }

    private async Task<bool> IsCpfAlreadyUsedAsync(string cpf)
    {
        return await localCacheManagerStrategy.IsKeyAlreadyUsedAsync($"CPF:{cpf}") ||
               await appDbContext.Users.AnyAsync(x => x.Cpf == cpf);
    }

    private async Task<bool> IsEmailAlreadyUsedAsync(string email)
    {
        return await localCacheManagerStrategy.IsKeyAlreadyUsedAsync($"Email:{email}") ||
               await appDbContext.Users.AnyAsync(x => x.Email == email);
    }

    private async Task<bool> IsPhoneNumberAlreadyUsedAsync(string phoneNumber)
    {
        return await localCacheManagerStrategy.IsKeyAlreadyUsedAsync($"PhoneNumber:{phoneNumber}") ||
               await appDbContext.Users.AnyAsync(x => x.PhoneNumber == phoneNumber);
    }
}