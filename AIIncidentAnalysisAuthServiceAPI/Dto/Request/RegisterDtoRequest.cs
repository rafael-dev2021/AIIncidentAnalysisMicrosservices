using AIIncidentAnalysisAuthServiceAPI.Models.Enums;

namespace AIIncidentAnalysisAuthServiceAPI.Dto.Request;

public record RegisterDtoRequest(
    string? Name,
    string? LastName,
    string? Email,
    string? PhoneNumber,
    string Cpf,
    DateTime DateOfBirth,
    DateTime DateOfJoining,
    ERank ERank,
    EDepartment EDepartment,
    EOfficerStatus EOfficerStatus,
    EAccessLevel EAccessLevel,
    string Password,
    string? ConfirmPassword);