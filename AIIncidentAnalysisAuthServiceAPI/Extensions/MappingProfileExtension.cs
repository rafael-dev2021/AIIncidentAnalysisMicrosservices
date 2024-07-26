using AIIncidentAnalysisAuthServiceAPI.Mapper;

namespace AIIncidentAnalysisAuthServiceAPI.Extensions;

public static class MappingProfileExtension
{
    public static void AddMappingProfileExtension(this IServiceCollection service) =>
        service.AddAutoMapper(typeof(MappingTheUserProfile));
}