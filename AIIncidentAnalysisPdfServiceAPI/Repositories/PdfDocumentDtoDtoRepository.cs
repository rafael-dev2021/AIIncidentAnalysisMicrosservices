using AIIncidentAnalysisPdfServiceAPI.Dto;
using AIIncidentAnalysisPdfServiceAPI.Repositories.Interfaces;
using AIIncidentAnalysisPdfServiceAPI.Utils;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AIIncidentAnalysisPdfServiceAPI.Repositories;

public class PdfDocumentDtoDtoRepository : IPdfDocumentDtoRepository
{
    private readonly IMongoCollection<PdfDocumentDto> _documents;

    public PdfDocumentDtoDtoRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _documents = database.GetCollection<PdfDocumentDto>(settings.Value.PdfCollection);
    }

    public async Task CreateAsync(PdfDocumentDto pdfDocument) =>
        await _documents.InsertOneAsync(pdfDocument);

    public async Task<List<PdfDocumentDto>> ListAllPdfDocumentsAsync() =>
        await _documents.Find(_ => true).ToListAsync();

    public async Task<PdfDocumentDto> GetPdfByIdAsync(string id) =>
        await _documents.Find(doc => doc.Id == id).FirstOrDefaultAsync();
}