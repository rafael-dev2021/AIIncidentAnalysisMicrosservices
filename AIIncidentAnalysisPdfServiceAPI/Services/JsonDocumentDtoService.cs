using System.Text.Json;
using AIIncidentAnalysisPdfServiceAPI.Dto;
using AIIncidentAnalysisPdfServiceAPI.Repositories.Interfaces;
using AIIncidentAnalysisPdfServiceAPI.Services.Interfaces;

namespace AIIncidentAnalysisPdfServiceAPI.Services;

public class JsonDocumentDtoService(
    IJsonDocumentDtoRepository jsonDocumentDtoRepository,
    IPdfDocumentDtoRepository pdfDocumentDtoRepository) : IJsonDocumentDtoService
{
    public async Task<string> ConvertPdfToJsonAsync(string id)
    {
        var pdfDocument = await pdfDocumentDtoRepository.GetPdfByIdAsync(id);

        if (pdfDocument == null)
            throw new ArgumentException("Document not found");

        // Codificar o conteúdo do PDF em Base64
        var base64Content = Convert.ToBase64String(pdfDocument.Content!);

        // Lógica de conversão do PDF para JSON
        var jsonContent = JsonSerializer.Serialize(new
        {
            pdfDocument.FileName,
            pdfDocument.ContentType,
            Content = base64Content,
            pdfDocument.UploadedAt
        });

        var jsonDocument = new JsonDocumentDto(
            pdfDocument.Id,
            pdfDocument.FileName,
            "application/json",
            jsonContent,
            DateTime.Now
        );

        await jsonDocumentDtoRepository.CreateJsonAsync(jsonDocument);

        return "PDF converted to JSON and saved successfully.";
    }

    public async Task<List<JsonDocumentDto>> ListAllJsonDocumentsAsync() =>
        await jsonDocumentDtoRepository.ListAllJsonDocumentsAsync();

    public async Task<JsonDocumentDto> GetJsonByIdAsync(string id) =>
        await jsonDocumentDtoRepository.GetJsonByIdAsync(id);

    public async Task<byte[]> DecodeJsonDocumentAsync(string id)
    {
        var jsonDocument = await jsonDocumentDtoRepository.GetJsonByIdAsync(id);

        if (jsonDocument == null)
            throw new ArgumentException("Document not found");

        var jsonData = JsonSerializer.Deserialize<JsonDocumentContentDto>(jsonDocument.JsonContent!);
        return Convert.FromBase64String(jsonData!.Content!);
    }
}