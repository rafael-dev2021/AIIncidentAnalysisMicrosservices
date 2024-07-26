using AIIncidentAnalysisAuthServiceAPI.Endpoints;
using AIIncidentAnalysisAuthServiceAPI.Extensions;
using AIIncidentAnalysisAuthServiceAPI.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureModule();
builder.Services.AddInfrastructureModuleStrategies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await UserRolesDataExtensions.AddUserRolesDataExtensions(app);

app.UseCors("CorsPolicy");

app.UseMiddleware<SecurityFilterMiddleware>();
app.UseMiddleware<LogoutHandlerMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthenticateEndpoints();

await app.RunAsync();


