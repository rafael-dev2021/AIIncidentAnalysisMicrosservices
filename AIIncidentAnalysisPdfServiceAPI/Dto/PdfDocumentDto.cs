using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AIIncidentAnalysisPdfServiceAPI.Dto;

public record PdfDocumentDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("fileName")] public string? FileName { get; init; }
    [BsonElement("contentType")] public string? ContentType { get; init; }
    [BsonElement("content")] public byte[]? Content { get; init; }
    [BsonElement("uploadedAt")] public DateTime UploadedAt { get; init; }
}
