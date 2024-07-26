namespace AIIncidentAnalysisAuthServiceAPI.Dto.Response;

public record UserDtoResponse(
    string Id,
    string IdentificationNumber,
    string Name,
    string LastName,
    string BadgeNumber,
    string Role,
    DateTime DateOfBirth,
    DateTime DateOfJoining,
    string ERank,
    string EDepartment,
    string EOfficerStatus,
    string EAccessLevel);