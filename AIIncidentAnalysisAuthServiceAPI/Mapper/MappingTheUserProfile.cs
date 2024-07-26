using AIIncidentAnalysisAuthServiceAPI.Dto.Response;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AutoMapper;

namespace AIIncidentAnalysisAuthServiceAPI.Mapper;

public class MappingTheUserProfile : Profile
{
    public MappingTheUserProfile()
    {
        CreateMap<PoliceOfficer, UserDtoResponse>()
            .ForMember(dest => dest.ERank, opt => opt.MapFrom(src => src.ERank.ToString()))
            .ForMember(dest => dest.EDepartment, opt => opt.MapFrom(src => src.EDepartment.ToString()))
            .ForMember(dest => dest.EOfficerStatus, opt => opt.MapFrom(src => src.EOfficerStatus.ToString()))
            .ForMember(dest => dest.EAccessLevel, opt => opt.MapFrom(src => src.EAccessLevel.ToString()));

        CreateMap<UserDtoResponse, PoliceOfficer>().ReverseMap();
    }
}