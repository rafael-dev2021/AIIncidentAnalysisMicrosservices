using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories;

public class TokenRepository(AppDbContext context, IMemoryCache cache) : ITokenRepository
{
    private const string TokenCacheKey = "UserTokens_";

    public async Task<List<Token>> FindAllValidTokenByUser(string userId)
    {
        if (cache.TryGetValue(TokenCacheKey + userId, out List<Token>? cachedTokens)) return cachedTokens!;

        cachedTokens = await context.Tokens
            .Where(t => t.UserId == userId && (!t.TokenExpired || !t.TokenRevoked))
            .ToListAsync();

        cache.Set(TokenCacheKey + userId, cachedTokens, TimeSpan.FromMinutes(5));

        return cachedTokens;
    }

    public async Task<List<Token>> FindAllTokensByUserId(string userId)
    {
        if (cache.TryGetValue(TokenCacheKey + userId, out List<Token>? cachedTokens)) return cachedTokens!;

        cachedTokens = await context.Tokens
            .Where(x => x.UserId == userId)
            .ToListAsync();

        cache.Set(TokenCacheKey + userId, cachedTokens, TimeSpan.FromMinutes(5));

        return cachedTokens;
    }

    public async Task<Token?> FindByTokenValue(string token) =>
        await context.Tokens.FirstOrDefaultAsync(t => t.TokenValue == token);

    public async Task DeleteByUser(PoliceOfficer policeOfficer)
    {
        var tokens = await context.Tokens
            .Where(t => t.UserId == policeOfficer.Id)
            .ToListAsync();

        context.Tokens.RemoveRange(tokens);
        await context.SaveChangesAsync();

        cache.Remove(TokenCacheKey + policeOfficer.Id);
    }

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task SaveAllTokensAsync(IEnumerable<Token> tokens)
    {
        var enumerable = tokens as Token[] ?? tokens.ToArray();

        foreach (var token in enumerable)
        {
            var existingToken = await context.Tokens.FindAsync(token.Id);
            if (existingToken == null)
            {
                context.Tokens.Add(token);
            }
            else
            {
                context.Entry(existingToken).CurrentValues.SetValues(token);
            }
        }

        await context.SaveChangesAsync();

        foreach (var token in enumerable)
        {
            cache.Remove(TokenCacheKey + token.UserId);
        }
    }

    public async Task DeleteAllTokensAsync(IEnumerable<Token> tokens)
    {
        var enumerable = tokens as Token[] ?? tokens.ToArray();

        context.Tokens.RemoveRange(enumerable);
        await context.SaveChangesAsync();

        foreach (var token in enumerable)
        {
            cache.Remove(TokenCacheKey + token.UserId);
        }
    }

    public async Task<Token> SaveTokenAsync(Token token)
    {
        var existingToken = await context.Tokens.FindAsync(token.Id);
        if (existingToken == null)
        {
            context.Tokens.Add(token);
        }
        else
        {
            context.Entry(existingToken).CurrentValues.SetValues(token);
        }

        await context.SaveChangesAsync();
        cache.Remove(TokenCacheKey + token.UserId);
        return token;
    }
}