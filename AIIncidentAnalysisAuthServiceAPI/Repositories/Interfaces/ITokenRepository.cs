using AIIncidentAnalysisAuthServiceAPI.Models;

namespace AIIncidentAnalysisAuthServiceAPI.Repositories.Interfaces;

public interface ITokenRepository
{
    Task<List<Token>> FindAllValidTokenByUser(string userId);
    Task<List<Token>> FindAllTokensByUserId(string userId);
    Task<Token?> FindByTokenValue(string token);
    Task DeleteByUser(PoliceOfficer policeOfficer);
    Task SaveAsync();
    Task SaveAllTokensAsync(IEnumerable<Token> tokens);
    Task DeleteAllTokensAsync(IEnumerable<Token> tokens);
    Task<Token> SaveTokenAsync(Token token);
}