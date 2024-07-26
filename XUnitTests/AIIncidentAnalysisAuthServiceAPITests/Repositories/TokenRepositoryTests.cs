using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using AIIncidentAnalysisAuthServiceAPI.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Repositories;

public class TokenRepositoryTests
{
    private static TokenRepository CreateTokenRepository(out AppDbContext dbContext, out IMemoryCache memoryCache)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        dbContext = new AppDbContext(options);
        memoryCache = new MemoryCache(new MemoryCacheOptions());
        return new TokenRepository(dbContext, memoryCache);
    }

    [Fact(DisplayName = "FindAllValidTokenByUser should return valid tokens for a user")]
    public async Task FindAllValidTokenByUser_Should_Return_Valid_Tokens()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext, out var memoryCache);
        const string userId = "1";
        const string cacheKey = "UserTokens_" + userId;

        var token1 = new Token();
        token1.SetUserId(userId);
        token1.SetTokenValue("Token1");
        token1.SetTokenExpired(false);
        token1.SetTokenRevoked(false);
        token1.SetTokenType(ETokenType.Bearer);

        var token2 = new Token();
        token2.SetUserId(userId);
        token2.SetTokenValue("Token2");
        token2.SetTokenExpired(false);
        token2.SetTokenRevoked(false);
        token2.SetTokenType(ETokenType.Bearer);

        var tokens = new List<Token> { token1, token2 };

        dbContext.Tokens.AddRange(tokens);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await tokenRepository.FindAllValidTokenByUser(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.TokenValue == "Token1");
        result.Should().Contain(t => t.TokenValue == "Token2");

        memoryCache.TryGetValue(cacheKey, out var cachedTokens).Should().BeTrue();
        var cachedTokenList = cachedTokens as List<Token>;
        cachedTokenList.Should().NotBeNull();
        cachedTokenList!.Should().HaveCount(2);
        cachedTokenList.Should().Contain(t => t.TokenValue == "Token1");
        cachedTokenList.Should().Contain(t => t.TokenValue == "Token2");
    }

    [Fact(DisplayName = "FindAllTokensByUserId should return all tokens for a user")]
    public async Task FindAllTokensByUserId_Should_Return_All_Tokens()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext, out var memoryCache);
        const string userId = "1";
        const string cacheKey = "UserTokens_" + userId;

        var token1 = new Token();
        token1.SetId(1);
        token1.SetUserId(userId);
        token1.SetTokenValue("Token1");
        token1.SetTokenExpired(false);
        token1.SetTokenRevoked(false);
        token1.SetTokenType(ETokenType.Bearer);

        var token2 = new Token();
        token2.SetId(2);
        token2.SetUserId(userId);
        token2.SetTokenValue("Token2");
        token2.SetTokenExpired(false);
        token2.SetTokenRevoked(false);
        token2.SetTokenType(ETokenType.Bearer);

        dbContext.Tokens.AddRange(token1, token2);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await tokenRepository.FindAllTokensByUserId(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.TokenValue == "Token1");
        result.Should().Contain(t => t.TokenValue == "Token2");

        memoryCache.TryGetValue(cacheKey, out var cachedTokens).Should().BeTrue();
        var cachedTokenList = cachedTokens as List<Token>;
        cachedTokenList.Should().NotBeNull();
        cachedTokenList!.Should().HaveCount(2);
        cachedTokenList.Should().Contain(t => t.TokenValue == "Token1");
        cachedTokenList.Should().Contain(t => t.TokenValue == "Token2");
    }

    [Fact(DisplayName = "FindByTokenValue should return token by its value")]
    public async Task FindByTokenValue_Should_Return_Token_By_Value()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext, out _);
        const string tokenValue = "Token1";

        var token = new Token();
        token.SetTokenValue(tokenValue);
        token.SetTokenExpired(false);
        token.SetTokenRevoked(false);
        token.SetTokenType(ETokenType.Bearer);

        dbContext.Tokens.Add(token);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await tokenRepository.FindByTokenValue(tokenValue);

        // Assert
        result.Should().NotBeNull();
        result!.TokenValue.Should().Be(tokenValue);
    }

    [Fact(DisplayName = "SaveAsync should persist changes to the database")]
    public async Task SaveAsync_Should_Persist_Changes_To_Database()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext, out _);
        var token = new Token();
        token.SetId(1);
        token.SetTokenValue("Token1");
        token.SetTokenExpired(false);
        token.SetTokenRevoked(false);
        token.SetTokenType(ETokenType.Bearer);

        dbContext.Tokens.Add(token);

        // Act
        await tokenRepository.SaveAsync();

        // Assert
        var savedToken = await dbContext.Tokens.FindAsync(token.Id);
        savedToken.Should().NotBeNull();
        savedToken!.TokenValue.Should().Be("Token1");
    }

    [Fact(DisplayName = "DeleteByUser should remove tokens and clear cache")]
    public async Task DeleteByUser_Should_Remove_Tokens_And_Clear_Cache()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext, out var memoryCache);
        const string userId = "1";
        const string cacheKey = "UserTokens_" + userId;

        var token1 = new Token();
        token1.SetId(1);
        token1.SetUserId(userId);
        token1.SetTokenValue("Token1");
        token1.SetTokenExpired(false);
        token1.SetTokenRevoked(false);
        token1.SetTokenType(ETokenType.Bearer);

        var token2 = new Token();
        token2.SetId(2);
        token2.SetUserId(userId);
        token2.SetTokenValue("Token2");
        token2.SetTokenExpired(false);
        token2.SetTokenRevoked(false);
        token2.SetTokenType(ETokenType.Bearer);

        dbContext.Tokens.AddRange(token1, token2);
        await dbContext.SaveChangesAsync();

        // Ensure tokens are in the database and cache
        var initialTokens = await dbContext.Tokens.ToListAsync();
        initialTokens.Should().HaveCount(2);

        memoryCache.Set(cacheKey, initialTokens, TimeSpan.FromMinutes(5));
        memoryCache.TryGetValue(cacheKey, out _).Should().BeTrue();

        // Act
        var policeOfficer = new PoliceOfficer { Id = userId };
        await tokenRepository.DeleteByUser(policeOfficer);

        // Assert - Verify tokens are removed from the database
        var remainingTokens = await dbContext.Tokens.ToListAsync();
        remainingTokens.Should().BeEmpty();

        // Verify cache is cleared
        memoryCache.TryGetValue(cacheKey, out _).Should().BeFalse();
    }

    [Fact(DisplayName = "SaveAllTokensAsync should add new tokens, update existing tokens, and clear cache")]
    public async Task SaveAllTokensAsync_Should_Add_New_Tokens_Update_Existing_Tokens_And_Clear_Cache()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext, out var memoryCache);
        const string userId = "1";
        const string cacheKey = "UserTokens_" + userId;

        // Prepare existing tokens and add them to the database
        var existingToken = new Token();
        existingToken.SetId(1);
        existingToken.SetUserId(userId);
        existingToken.SetTokenValue("ExistingToken");
        existingToken.SetTokenExpired(false);
        existingToken.SetTokenRevoked(false);
        existingToken.SetTokenType(ETokenType.Bearer);

        dbContext.Tokens.Add(existingToken);
        await dbContext.SaveChangesAsync();

        // Cache the existing token for verification
        memoryCache.Set(cacheKey, new List<Token> { existingToken }, TimeSpan.FromMinutes(5));
        memoryCache.TryGetValue(cacheKey, out _).Should().BeTrue();

        // Prepare new tokens and updated tokens
        var newToken = new Token();
        newToken.SetUserId(userId);
        newToken.SetTokenValue("NewToken");
        newToken.SetTokenExpired(false);
        newToken.SetTokenRevoked(false);
        newToken.SetTokenType(ETokenType.Bearer);

        var updatedToken = new Token();
        updatedToken.SetId(1);
        updatedToken.SetUserId(userId);
        updatedToken.SetTokenValue("UpdatedToken");
        updatedToken.SetTokenExpired(false);
        updatedToken.SetTokenRevoked(false);
        existingToken.SetTokenType(ETokenType.Bearer);

        var tokens = new List<Token> { newToken, updatedToken };

        // Act
        await tokenRepository.SaveAllTokensAsync(tokens);

        // Assert - Verify tokens are added or updated in the database
        var savedTokens = await dbContext.Tokens.ToListAsync();
        savedTokens.Should().HaveCount(2);
        savedTokens.Should().Contain(t => t.TokenValue == "NewToken");
        savedTokens.Should().Contain(t => t.TokenValue == "UpdatedToken");

        // Verify cache is cleared
        memoryCache.TryGetValue(cacheKey, out _).Should().BeFalse();
    }

    [Fact(DisplayName = "DeleteAllTokensAsync should remove all tokens and clear cache")]
    public async Task DeleteAllTokensAsync_Should_Remove_All_Tokens_And_Clear_Cache()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext, out var memoryCache);
        const string userId = "1";
        const string cacheKey = "UserTokens_" + userId;

        var token1 = new Token();
        token1.SetId(1);
        token1.SetUserId(userId);
        token1.SetTokenValue("Token1");
        token1.SetTokenExpired(false);
        token1.SetTokenRevoked(false);
        token1.SetTokenType(ETokenType.Bearer);

        var token2 = new Token();
        token2.SetId(2);
        token2.SetUserId(userId);
        token2.SetTokenValue("Token2");
        token2.SetTokenExpired(false);
        token2.SetTokenRevoked(false);
        token2.SetTokenType(ETokenType.Bearer);

        var tokens = new List<Token> { token1, token2 };

        dbContext.Tokens.AddRange(tokens);
        await dbContext.SaveChangesAsync();

        // Cache the tokens
        memoryCache.Set(cacheKey, tokens, TimeSpan.FromMinutes(5));
        memoryCache.TryGetValue(cacheKey, out _).Should().BeTrue();

        // Act
        await tokenRepository.DeleteAllTokensAsync(tokens);

        // Assert - Verify tokens are removed from the database
        var remainingTokens = await dbContext.Tokens.ToListAsync();
        remainingTokens.Should().BeEmpty();

        // Verify cache is cleared
        memoryCache.TryGetValue(cacheKey, out _).Should().BeFalse();
    }

    [Fact(DisplayName = "SaveTokenAsync should add a new token and clear cache")]
    public async Task SaveTokenAsync_Should_Add_New_Token_And_Clear_Cache()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext, out var memoryCache);
        const string userId = "1";
        const string cacheKey = "UserTokens_" + userId;

        var newToken = new Token();
        newToken.SetId(1);
        newToken.SetUserId(userId);
        newToken.SetTokenValue("NewToken");
        newToken.SetTokenExpired(false);
        newToken.SetTokenRevoked(false);
        newToken.SetTokenType(ETokenType.Bearer);

        // Act
        await tokenRepository.SaveTokenAsync(newToken);

        // Assert
        var savedToken = await dbContext.Tokens.FindAsync(newToken.Id);
        savedToken.Should().NotBeNull();
        savedToken!.TokenValue.Should().Be("NewToken");

        memoryCache.TryGetValue(cacheKey, out _).Should().BeFalse();
    }

    [Fact(DisplayName = "SaveTokenAsync should update an existing token and clear cache")]
    public async Task SaveTokenAsync_Should_Update_Existing_Token_And_Clear_Cache()
    {
        // Arrange
        var tokenRepository = CreateTokenRepository(out var dbContext, out var memoryCache);
        const string userId = "1";
        const string cacheKey = "UserTokens_" + userId;

        var existingToken = new Token();
        existingToken.SetId(1);
        existingToken.SetUserId(userId);
        existingToken.SetTokenValue("ExistingToken");
        existingToken.SetTokenExpired(false);
        existingToken.SetTokenRevoked(false);
        existingToken.SetTokenType(ETokenType.Bearer);

        dbContext.Tokens.Add(existingToken);
        await dbContext.SaveChangesAsync();

        var updatedToken = new Token();
        updatedToken.SetId(1);
        updatedToken.SetUserId(userId);
        updatedToken.SetTokenValue("UpdatedToken");
        updatedToken.SetTokenExpired(false);
        updatedToken.SetTokenRevoked(false);
        existingToken.SetTokenType(ETokenType.Bearer);

        // Act
        await tokenRepository.SaveTokenAsync(updatedToken);

        // Assert
        var savedToken = await dbContext.Tokens.FindAsync(updatedToken.Id);
        savedToken.Should().NotBeNull();
        savedToken!.TokenValue.Should().Be("UpdatedToken");

        memoryCache.TryGetValue(cacheKey, out _).Should().BeFalse();
    }
}