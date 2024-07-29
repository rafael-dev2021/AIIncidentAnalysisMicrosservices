using AIIncidentAnalysisPdfServiceAPI.Dto;

namespace AIIncidentAnalysisPdfServiceAPI.Services.Interfaces;

public interface IPdfDocumentService
{
    Task<string> UploadAsync(IFormFile file);
    Task<List<PdfDocumentDto>> ListAllPdfDocumentsAsync();
    Task<PdfDocumentDto> GetPdfByIdAsync(string id);
}