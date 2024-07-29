using System.Net.Http.Headers;
using AIIncidentAnalysisDocsFrontend.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AIIncidentAnalysisDocsFrontend.Controllers;

public class PdfController(IConfiguration configuration) : Controller
{
    private readonly string _apiUrl = configuration["ApiPdfDocumentUrl"]!;

    public async Task<IActionResult> Index()
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"{_apiUrl}/documents");

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            var documents = JsonConvert.DeserializeObject<List<PdfDocumentDto>>(responseData);
            return View(documents);
        }

        ViewBag.Message = "Falha ao carregar os documentos.";
        return View(new List<PdfDocumentDto>());
    }

    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file.Length == 0)
        {
            ViewBag.Message = "Por favor, selecione um arquivo.";
            return View();
        }

        using var client = new HttpClient();
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(file.OpenReadStream())
        {
            Headers = { ContentLength = file.Length, ContentType = new MediaTypeHeaderValue(file.ContentType) }
        }, "file", file.FileName);

        var response = await client.PostAsync($"{_apiUrl}/upload", content);

        if (response.IsSuccessStatusCode)
        {
            ViewBag.Message = "Arquivo enviado com sucesso.";
        }
        else
        {
            ViewBag.Message = "Falha no upload: " + await response.Content.ReadAsStringAsync();
        }

        return View();
    }

    public async Task<IActionResult> Download(string id)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"{_apiUrl}/documents/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }

        var content = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/pdf";
        var contentDisposition = response.Content.Headers.ContentDisposition;
        var fileName = contentDisposition?.FileNameStar ?? contentDisposition?.FileName ?? "file.pdf";

        return File(content, contentType, fileName);
    }
}