using AIIncidentAnalysisPdfServiceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIIncidentAnalysisPdfServiceAPI.Endpoints;

public static class MapPdfDocuments
{
    public static void MapPdfDocumentsEndpoints(this WebApplication app)
    {
        app.MapPost("/api/v1/pdf/upload",
            async (HttpRequest request, [FromServices] IPdfDocumentService pdfDocumentService) =>
            {
                if (!request.HasFormContentType)
                {
                    return Results.BadRequest("Content type must be 'multipart/form-data'.");
                }

                var form = await request.ReadFormAsync();
                var file = form.Files.GetFile("file");

                if (file == null || file.Length == 0)
                {
                    return Results.BadRequest("No file uploaded.");
                }

                try
                {
                    var result = await pdfDocumentService.UploadAsync(file);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

        app.MapGet("/api/v1/pdf/documents", async ([FromServices] IPdfDocumentService pdfDocumentService) =>
        {
            var documents = await pdfDocumentService.ListAllPdfDocumentsAsync();
            return Results.Ok(documents);
        });

        app.MapGet("/api/v1/pdf/documents/{id}",
            async (string id, [FromServices] IPdfDocumentService pdfDocumentService) =>
            {
                var document = await pdfDocumentService.GetPdfByIdAsync(id);
                return Results.File(document.Content!, document.ContentType, document.FileName);
            });
    }
}