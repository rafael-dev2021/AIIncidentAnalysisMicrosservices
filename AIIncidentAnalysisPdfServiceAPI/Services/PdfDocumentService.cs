using AIIncidentAnalysisPdfServiceAPI.Dto;
using AIIncidentAnalysisPdfServiceAPI.Repositories.Interfaces;
using AIIncidentAnalysisPdfServiceAPI.Services.Interfaces;

namespace AIIncidentAnalysisPdfServiceAPI.Services;

public class PdfDocumentService(IPdfDocumentDtoRepository dtoRepository) : IPdfDocumentService
{
    public async Task<string> UploadAsync(IFormFile file)
    {
        if (file.Length == 0)
            throw new ArgumentException("No file uploaded");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var document = new PdfDocumentDto
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Content = memoryStream.ToArray(),
            UploadedAt = DateTime.Now
        };

        await dtoRepository.CreateAsync(document);

        return "File uploaded successfully.";
    }

    public async Task<List<PdfDocumentDto>> ListAllPdfDocumentsAsync() =>
        await dtoRepository.ListAllPdfDocumentsAsync();

    public async Task<PdfDocumentDto> GetPdfByIdAsync(string id) =>
        await dtoRepository.GetPdfByIdAsync(id);
}