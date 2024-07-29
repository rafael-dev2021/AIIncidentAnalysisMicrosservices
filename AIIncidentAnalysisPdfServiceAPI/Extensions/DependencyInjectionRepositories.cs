using AIIncidentAnalysisPdfServiceAPI.Repositories;
using AIIncidentAnalysisPdfServiceAPI.Repositories.Interfaces;

namespace AIIncidentAnalysisPdfServiceAPI.Extensions;

public static class DependencyInjectionRepositories
{
    public static void AddDependencyInjectionRepositories(this IServiceCollection service)
    {
        service.AddSingleton<IPdfDocumentDtoRepository, PdfDocumentDtoDtoRepository>();
        service.AddSingleton<IJsonDocumentDtoRepository, JsonDocumentDtoRepositoryRepository>();
    }
}