using AIIncidentAnalysisPdfServiceAPI.Dto;

namespace AIIncidentAnalysisPdfServiceAPI.Repositories.Interfaces;

public interface IJsonDocumentDtoRepository
{
    Task CreateJsonAsync(JsonDocumentDto jsonDocument);
    Task<List<JsonDocumentDto>> ListAllJsonDocumentsAsync();
    Task<JsonDocumentDto> GetJsonByIdAsync(string id);
}