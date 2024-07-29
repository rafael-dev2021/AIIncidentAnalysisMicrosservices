using AIIncidentAnalysisPdfServiceAPI.Services;
using AIIncidentAnalysisPdfServiceAPI.Services.Interfaces;

namespace AIIncidentAnalysisPdfServiceAPI.Extensions;

public static class DependencyInjectionServices
{
    public static void AddDependencyInjectionServices(this IServiceCollection service)
    {
        service.AddSingleton<IPdfDocumentService, PdfDocumentService>();
        service.AddSingleton<IJsonDocumentDtoService, JsonDocumentDtoService>();
    }
}