using AIIncidentAnalysisPdfServiceAPI.Dto;
using AIIncidentAnalysisPdfServiceAPI.Repositories.Interfaces;
using AIIncidentAnalysisPdfServiceAPI.Utils;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AIIncidentAnalysisPdfServiceAPI.Repositories;

public class JsonDocumentDtoRepositoryRepository : IJsonDocumentDtoRepository
{
    private readonly IMongoCollection<JsonDocumentDto> _repository;

    public JsonDocumentDtoRepositoryRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _repository = database.GetCollection<JsonDocumentDto>(settings.Value.JsonPdfCollection);
    }

    public async Task CreateJsonAsync(JsonDocumentDto jsonDocument) =>
        await _repository.InsertOneAsync(jsonDocument);

    public async Task<List<JsonDocumentDto>> ListAllJsonDocumentsAsync() =>
        await _repository.Find(_ => true).ToListAsync();

    public async Task<JsonDocumentDto> GetJsonByIdAsync(string id) =>
        await _repository.Find(doc => doc.Id == id).FirstOrDefaultAsync();
}