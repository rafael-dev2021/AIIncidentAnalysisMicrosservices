using AIIncidentAnalysisAuthServiceAPI.Models.Enums;

namespace AIIncidentAnalysisAuthServiceAPI.Models;

public class UserDetails
{
    public string? IdentificationNumber { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? BadgeNumber { get; set; }
    public string? Cpf { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Role { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime DateOfJoining { get; set; }
    public ERank ERank { get; set; }
    public EDepartment EDepartment { get; set; }
    public EOfficerStatus EOfficerStatus { get; set; }
    public EAccessLevel EAccessLevel { get; set; }
}