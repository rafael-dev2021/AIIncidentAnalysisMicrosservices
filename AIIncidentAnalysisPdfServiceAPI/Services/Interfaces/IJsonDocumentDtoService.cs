using AIIncidentAnalysisPdfServiceAPI.Dto;

namespace AIIncidentAnalysisPdfServiceAPI.Services.Interfaces;

public interface IJsonDocumentDtoService
{
    Task<List<JsonDocumentDto>> ListAllJsonDocumentsAsync();
    Task<string> ConvertPdfToJsonAsync(string id);
    Task<JsonDocumentDto> GetJsonByIdAsync(string id);
    Task<byte[]> DecodeJsonDocumentAsync(string id);
}