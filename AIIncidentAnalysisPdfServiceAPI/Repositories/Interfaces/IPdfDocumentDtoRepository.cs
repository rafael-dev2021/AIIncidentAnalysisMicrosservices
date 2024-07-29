using AIIncidentAnalysisPdfServiceAPI.Dto;

namespace AIIncidentAnalysisPdfServiceAPI.Repositories.Interfaces;

public interface IPdfDocumentDtoRepository
{
    Task CreateAsync(PdfDocumentDto pdfDocument);
    Task<List<PdfDocumentDto>> ListAllPdfDocumentsAsync();
    Task<PdfDocumentDto> GetPdfByIdAsync(string id);
}