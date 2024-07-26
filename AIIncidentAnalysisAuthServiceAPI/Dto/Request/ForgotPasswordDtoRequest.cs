namespace AIIncidentAnalysisAuthServiceAPI.Dto.Request;

public record ForgotPasswordDtoRequest(string? Email, string? NewPassword);