using AIIncidentAnalysisAuthServiceAPI.Algorithms.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Context;
using Microsoft.EntityFrameworkCore;

namespace AIIncidentAnalysisAuthServiceAPI.Algorithms;

public class AccountNumberGenerator(AppDbContext context) : IAccountNumberGenerator
{
    private static readonly Random Random = new();

    public async Task<string> GenerateIdentificationNumberAsync()
    {
        return await GenerateUniqueIdentifierAsync(5, 15);
    }

    public async Task<string> GenerateBadgeNumberAsync()
    {
        return await GenerateUniqueIdentifierAsync(5, 10);
    }

    private async Task<string> GenerateUniqueIdentifierAsync(int minLength, int maxLength)
    {
        while (true)
        {
            var identifier = GenerateRandomString(minLength, maxLength);

            var exists = await context.Users.AnyAsync(x =>
                x.IdentificationNumber == identifier ||
                x.BadgeNumber == identifier);

            if (!exists) return identifier;
        }
    }

    private static string GenerateRandomString(int minLength, int maxLength)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var length = Random.Next(minLength, maxLength + 1);

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}