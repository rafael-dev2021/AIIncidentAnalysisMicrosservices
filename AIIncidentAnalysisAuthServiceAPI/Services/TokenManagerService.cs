using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;
using AIIncidentAnalysisAuthServiceAPI.Services.Interfaces;
using Serilog;

namespace AIIncidentAnalysisAuthServiceAPI.Services;

public class TokenManagerService(
    ITokenService tokenService,
    ITokenRepository tokenRepository,
    IAuthRepository authenticatedRepository) : ITokenManagerService
{
    public async Task<TokenDtoResponse> GenerateTokenResponseAsync(PoliceOfficer policeOfficer)
    {
        var accessToken = await tokenService.GenerateAccessTokenAsync(policeOfficer);
        var refreshToken = await tokenService.GenerateRefreshTokenAsync(policeOfficer);

        await ClearTokensAsync(policeOfficer.Id);
        var accessTokenToken = await SaveUserTokenAsync(policeOfficer, accessToken);
        var refreshTokenToken = await SaveUserTokenAsync(policeOfficer, refreshToken);

        policeOfficer.Tokens.Clear();
        policeOfficer.Tokens.Add(accessTokenToken);
        policeOfficer.Tokens.Add(refreshTokenToken);
        await authenticatedRepository.SaveUserAsync(policeOfficer);

        Log.Information("[TOKENS] Tokens generated successfully for user [{UserId}]", policeOfficer.Id);

        return new TokenDtoResponse(accessTokenToken.TokenValue!, refreshTokenToken.TokenValue!);
    }

    public void RevokeAllUserTokens(PoliceOfficer policeOfficer)
    {
        var validUserTokens = tokenRepository.FindAllValidTokenByUser(policeOfficer.Id).Result;
        if (validUserTokens.Count == 0) return;
        foreach (var token in validUserTokens)
        {
            token.SetTokenExpired(true);
            token.SetTokenRevoked(true);
        }

        tokenRepository.SaveAllTokensAsync(validUserTokens).Wait();

        Log.Information("[TOKENS] All tokens revoked for user [{UserId}]", policeOfficer.Id);
    }

    public async Task<bool> RevokedTokenAsync(string token)
    {
        var dbToken = await tokenRepository.FindByTokenValue(token);
        return dbToken is { TokenRevoked: true };
    }

    public async Task<bool> ExpiredTokenAsync(string token)
    {
        var dbToken = await tokenRepository.FindByTokenValue(token);
        return dbToken is { TokenExpired: true };
    }

    public async Task ClearTokensAsync(string userId)
    {
        var tokens = await tokenRepository.FindAllTokensByUserId(userId);
        if (tokens.Count == 0) return;
        await tokenRepository.DeleteAllTokensAsync(tokens);
        Log.Information("[CLEARED TOKENS] Cleared tokens for user [{UserId}]", userId);
    }

    public async Task<Token> SaveUserTokenAsync(PoliceOfficer policeOfficer, string jwtToken)
    {
        var token = new Token();
        token.SetTokenValue(jwtToken);
        token.SetTokenType(ETokenType.Bearer);
        token.SetTokenExpired(false);
        token.SetTokenRevoked(false);
        token.SetUserId(policeOfficer.Id);
        token.SetUser(policeOfficer);

        var savedToken = await tokenRepository.SaveTokenAsync(token);
        Log.Information("[TOKENS] Token saved successfully for user [{UserId}]", policeOfficer.Id);

        return savedToken;
    }
}