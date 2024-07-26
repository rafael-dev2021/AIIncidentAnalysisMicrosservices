using AIIncidentAnalysisAuthServiceAPI.Models.Enums;

namespace AIIncidentAnalysisAuthServiceAPI.Models;

public class Token
{
    public int Id { get; private set; }
    public string? TokenValue { get; private set; }
    public bool TokenRevoked { get; private set; }
    public bool TokenExpired { get; private set; }
    public ETokenType ETokenType { get; private set; }
    public string? UserId { get; private set; }
    public PoliceOfficer? PoliceOfficer { get; private set; }
    
    public void SetId(int id) => Id = id;
    public void SetTokenValue(string? tokenValue) => TokenValue = tokenValue;
    public void SetTokenRevoked(bool tokenRevoked) => TokenRevoked = tokenRevoked;
    public void SetTokenExpired(bool tokenExpired) => TokenExpired = tokenExpired;
    public void SetTokenType(ETokenType tokenType) => ETokenType = tokenType;
    public void SetUserId(string? userId) => UserId = userId;
    public void SetUser(PoliceOfficer? policeOfficer) => PoliceOfficer = policeOfficer;
}