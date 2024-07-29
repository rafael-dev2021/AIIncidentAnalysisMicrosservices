using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AIIncidentAnalysisPdfServiceAPI.Dto;

public record JsonDocumentDto(
    [property: BsonId] [property: BsonRepresentation(BsonType.ObjectId)] string? Id,
    [property: BsonElement("fileName")] string? FileName,
    [property: BsonElement("contentType")] string? ContentType,
    [property: BsonElement("jsonContent")] string? JsonContent,
    [property: BsonElement("UploadedAt")] DateTime UploadedAt
);