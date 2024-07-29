using AIIncidentAnalysisPdfServiceAPI.Endpoints;
using AIIncidentAnalysisPdfServiceAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureModule(builder.Configuration);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapPdfDocumentsEndpoints();
app.MapJsonPdfDocumentsEndpoints();

app.Run();