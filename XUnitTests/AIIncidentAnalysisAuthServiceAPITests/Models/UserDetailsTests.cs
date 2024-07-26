using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Models;

public class UserDetailsTests
{
    private readonly UserDetails _model = new();

    [Fact]
    public void Should_Not_Have_Validation_Errors_For_Valid_Model()
    {
        // Arrange
        _model.IdentificationNumber = "12345";
        _model.Name = "John Doe";
        _model.LastName = "John Doe";
        _model.BadgeNumber = "S123456";
        _model.Email = "john.doe@example.com";
        _model.PhoneNumber = "+123456789";
        _model.Role = "Admin";
        _model.Cpf = "123.456.789-01";
        _model.DateOfBirth = DateTime.Now.AddYears(-30);
        _model.DateOfJoining = DateTime.Now.AddYears(-1);
        _model.ERank = ERank.Sergeant;
        _model.EDepartment = EDepartment.TrafficDivision;
        _model.EOfficerStatus = EOfficerStatus.Active;
        _model.EAccessLevel = EAccessLevel.Admin;

        // Act & Assert
        Assert.False(string.IsNullOrEmpty(_model.IdentificationNumber));
        Assert.False(string.IsNullOrEmpty(_model.Name));
        Assert.False(string.IsNullOrEmpty(_model.LastName));
        Assert.False(string.IsNullOrEmpty(_model.BadgeNumber));
        Assert.False(string.IsNullOrEmpty(_model.Email));
        Assert.False(string.IsNullOrEmpty(_model.PhoneNumber));
        Assert.False(string.IsNullOrEmpty(_model.Role));
        Assert.False(string.IsNullOrEmpty(_model.Cpf));
        Assert.True(_model.DateOfBirth < DateTime.Now);
        Assert.True(_model.DateOfJoining < DateTime.Now);
        Assert.True(Enum.IsDefined(typeof(ERank), _model.ERank));
        Assert.True(Enum.IsDefined(typeof(EDepartment), _model.EDepartment));
        Assert.True(Enum.IsDefined(typeof(EOfficerStatus), _model.EOfficerStatus));
        Assert.True(Enum.IsDefined(typeof(EAccessLevel), _model.EAccessLevel));
    }
}