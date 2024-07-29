namespace AIIncidentAnalysisPdfServiceAPI.Utils;

public class MongoDbSettings
{
    public string ConnectionString { get; init; } = null!;
    public string DatabaseName { get; init; } = null!;
    public string PdfCollection { get; init; } = null!;
    public string JsonPdfCollection { get; init; } = null!;
}