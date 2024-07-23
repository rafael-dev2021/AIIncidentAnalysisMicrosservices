using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace AIIncidentAnalysisAuthServiceAPI.Models;

public class PoliceOfficer : IdentityUser
{
    public string? IdentificationNumber { get; private set; }
    public string? Name { get; private set; }
    public string? LastName { get; private set; }
    public string? BadgeNumber { get; private set; }
    public string? Cpf { get; private set; }
    public string? Role { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public DateTime DateOfJoining { get; private set; }
    public ERank ERank { get; private set; }
    public EDepartment EDepartment { get; private set; }
    public EOfficerStatus EOfficerStatus { get; private set; }
    public EAccessLevel EAccessLevel { get; private set; }
    public ICollection<Token> Tokens { get; } = [];
    
    public void SetIdentificationNumber(string? identificationNumber) => IdentificationNumber = identificationNumber;
    public void SetName(string? name) => Name = name;
    public void SetLastName(string? lastName) => LastName = lastName;
    public void SetPhoneNumber(string? phoneNumber) => PhoneNumber = phoneNumber;
    public void SetBadgeNumber(string? badgeNumber) => BadgeNumber = badgeNumber;
    public void SetEmail(string? email) => Email = email;
    public void SetCpf(string? cpf) => Cpf = cpf;
    public void SetRole(string? role) => Role = role;
    public void SetDateOfBirth(DateTime dateOfBirth) => DateOfBirth = dateOfBirth;
    public void SetDateOfJoining(DateTime dateOfJoining) => DateOfJoining = dateOfJoining;
    public void SetERank(ERank rank) => ERank = rank;
    public void SetEDepartment(EDepartment department) => EDepartment = department;
    public void SetEOfficerStatus(EOfficerStatus status) => EOfficerStatus = status;
    public void SetEAccessLevel(EAccessLevel accessLevel) => EAccessLevel = accessLevel;
    public void AddToken(Token token) => Tokens.Add(token);
}