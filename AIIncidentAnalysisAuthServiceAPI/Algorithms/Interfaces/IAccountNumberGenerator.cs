namespace AIIncidentAnalysisAuthServiceAPI.Algorithms.Interfaces;

public interface IAccountNumberGenerator
{
    Task<string> GenerateIdentificationNumberAsync();
    Task<string> GenerateBadgeNumberAsync();
}