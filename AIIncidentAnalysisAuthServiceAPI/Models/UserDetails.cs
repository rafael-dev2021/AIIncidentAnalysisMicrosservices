using AIIncidentAnalysisAuthServiceAPI.Models.Enums;

namespace AIIncidentAnalysisAuthServiceAPI.Models;

public class UserDetails
{
    public string? IdentificationNumber { get; init; }
    public string? Name { get; init; }
    public string? LastName { get; init; }
    public string? BadgeNumber { get; init; }
    public string? Cpf { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Role { get; set; }
    public DateTime DateOfBirth { get; init; }
    public DateTime DateOfJoining { get; init; }
    public ERank ERank { get; init; }
    public EDepartment EDepartment { get; init; }
    public EOfficerStatus EOfficerStatus { get; init; }
    public EAccessLevel EAccessLevel { get; init; }
}