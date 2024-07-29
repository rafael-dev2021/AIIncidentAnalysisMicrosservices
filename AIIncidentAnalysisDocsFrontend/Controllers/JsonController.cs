using System.Text;
using AIIncidentAnalysisDocsFrontend.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AIIncidentAnalysisDocsFrontend.Controllers;

public class JsonController(IConfiguration configuration) : Controller
{
    private readonly string _apiUrl = configuration["ApiJsonPdfDocumentUrl"]!;

    // GET
    public async Task<IActionResult> Index()
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"{_apiUrl}/json/documents");

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            var documents = JsonConvert.DeserializeObject<List<JsonDocumentDto>>(responseData);
            return View(documents);
        }

        ViewBag.Message = "Falha ao carregar os documentos.";
        return View(new List<JsonDocumentDto>());
    }

    public IActionResult ConvertPdf()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ConvertPdf(string id)
    {
        using var client = new HttpClient();
        var response = await client.PostAsync($"{_apiUrl}/pdf/convert/{id}", null);

        if (response.IsSuccessStatusCode)
        {
            ViewBag.Message = "PDF convertido para JSON com sucesso.";
        }
        else
        {
            ViewBag.Message = "Falha na conversão: " + await response.Content.ReadAsStringAsync();
        }

        return View();
    }

    public async Task<IActionResult> Download(string id)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"{_apiUrl}/json/documents/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }

        var content = await response.Content.ReadAsStringAsync();
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";
        var contentDisposition = response.Content.Headers.ContentDisposition;
        var fileName = contentDisposition?.FileNameStar ?? contentDisposition?.FileName ?? "file.json";

        return File(Encoding.UTF8.GetBytes(content), contentType, fileName);
    }

    // New method to decode JSON to PDF
    public async Task<IActionResult> DecodePdf(string id)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"{_apiUrl}/json/decode/{id}");

        if (response.IsSuccessStatusCode)
        {
            var pdfBytes = await response.Content.ReadAsByteArrayAsync();
            return File(pdfBytes, "application/pdf", $"{id}.pdf");
        }

        ViewBag.Message = "Falha ao decodificar o PDF: " + await response.Content.ReadAsStringAsync();
        return View();
    }

    // New method to verify the Base64 string
    public async Task<IActionResult> VerifyJson(string id)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"{_apiUrl}/json/verify/{id}");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            ViewBag.Message = "Verificação concluída: " + result;
        }
        else
        {
            ViewBag.Message = "Falha na verificação: " + await response.Content.ReadAsStringAsync();
        }

        return View();
    }
    
    public async Task<IActionResult> ExtractProperties(string id)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"{_apiUrl}/json/properties/{id}");

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            var properties = JsonConvert.DeserializeObject<PdfPropertiesDto>(responseData);
            return View(properties);
        }

        ViewBag.Message = "Falha ao extrair propriedades do PDF: " + await response.Content.ReadAsStringAsync();
        return View();
    }
}
