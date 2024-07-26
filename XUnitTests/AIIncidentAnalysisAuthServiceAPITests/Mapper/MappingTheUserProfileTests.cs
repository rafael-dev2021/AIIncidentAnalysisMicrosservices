using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Mapper;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Models.Enums;
using AutoMapper;

namespace XUnitTests.AIIncidentAnalysisAuthServiceAPITests.Mapper;

public class MappingTheUserProfileTests
{
    private readonly IMapper _mapper;

    public MappingTheUserProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile<MappingTheUserProfile>());

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_PoliceOfficer_To_UserDtoResponse()
    {
        // Arrange
        var policeOfficer = new PoliceOfficer
        {
            Id = "123",
            Email = "john.doe@example.com"
        };
        policeOfficer.SetName("John");
        policeOfficer.SetLastName("Doe");
        policeOfficer.SetCpf("123.456.789-10");
        policeOfficer.SetPhoneNumber("+5540028922");
        policeOfficer.SetERank(ERank.Captain);
        policeOfficer.SetEDepartment(EDepartment.CommunityPolicing);
        policeOfficer.SetEOfficerStatus(EOfficerStatus.Active);
        policeOfficer.SetEAccessLevel(EAccessLevel.Admin);

        // Act
        var result = _mapper.Map<UserDtoResponse>(policeOfficer);

        // Assert
        Assert.Equal(policeOfficer.Id, result.Id);
        Assert.Equal(policeOfficer.Name, result.Name);
        Assert.Equal(policeOfficer.LastName, result.LastName);
        Assert.Equal(policeOfficer.Role, result.Role);
        Assert.Equal(policeOfficer.ERank.ToString(), result.ERank);
        Assert.Equal(policeOfficer.EDepartment.ToString(), result.EDepartment);
        Assert.Equal(policeOfficer.EOfficerStatus.ToString(), result.EOfficerStatus);
        Assert.Equal(policeOfficer.EAccessLevel.ToString(), result.EAccessLevel);
    }

    [Fact]
    public void Should_Map_UserDtoResponse_To_PoliceOfficer()
    {
        // Arrange
        var userDto = new UserDtoResponse(
            "123",
            "ID123",
            "John",
            "Doe",
            "BADGE123",
            "Admin",
            new DateTime(1985, 1, 1),
            new DateTime(2010, 1, 1),
            "Captain",
            "CommunityPolicing",
            "Active",
            "Admin"
        );

        // Act
        var result = _mapper.Map<PoliceOfficer>(userDto);

        // Assert
        Assert.Equal(userDto.Id, result.Id);
        Assert.Equal(userDto.Name, result.Name);
        Assert.Equal(userDto.LastName, result.LastName);
        Assert.Equal(userDto.Role, result.Role);
        Assert.Equal(userDto.ERank, result.ERank.ToString());
        Assert.Equal(userDto.EDepartment, result.EDepartment.ToString());
        Assert.Equal(userDto.EOfficerStatus, result.EOfficerStatus.ToString());
        Assert.Equal(userDto.EAccessLevel, result.EAccessLevel.ToString());
    }
}