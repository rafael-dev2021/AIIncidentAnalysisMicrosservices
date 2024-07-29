using AIIncidentAnalysisPdfServiceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIIncidentAnalysisPdfServiceAPI.Endpoints;

public static class MapJsonPdfDocuments
{
    public static void MapJsonPdfDocumentsEndpoints(this WebApplication app)
    {
        app.MapPost("/api/v1/pdf/convert/{id}",
            async (string id, [FromServices] IJsonDocumentDtoService jsonDocumentDtoService) =>
            {
                try
                {
                    var result = await jsonDocumentDtoService.ConvertPdfToJsonAsync(id);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

        app.MapGet("/api/v1/json/documents", async ([FromServices] IJsonDocumentDtoService jsonDocumentDtoService) =>
        {
            var documents = await jsonDocumentDtoService.ListAllJsonDocumentsAsync();
            return Results.Ok(documents);
        });

        app.MapGet("/api/v1/json/documents/{id}",
            async (string id, [FromServices] IJsonDocumentDtoService jsonDocumentDtoService) =>
            {
                var document = await jsonDocumentDtoService.GetJsonByIdAsync(id);
                return Results.Ok(document);
            });
        
        app.MapGet("/api/v1/json/decode/{id}", async (string id, [FromServices] IJsonDocumentDtoService jsonDocumentDtoService) =>
        {
            try
            {
                var pdfBytes = await jsonDocumentDtoService.DecodeJsonDocumentAsync(id);
                return Results.File(pdfBytes, "application/pdf", $"{id}.pdf");
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });
        
        app.MapGet("/api/v1/json/verify/{id}", async (string id, [FromServices] IJsonDocumentDtoService jsonDocumentDtoService) =>
        {
            try
            {
                var pdfBytes = await jsonDocumentDtoService.DecodeJsonDocumentAsync(id);
                return Results.Ok(new { Message = "PDF successfully decoded", PdfLength = pdfBytes.Length });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });
    }
}