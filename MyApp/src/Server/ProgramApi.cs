using MyApp.Infrastructure.Logging;
using MyApp.Presentation;
using MyApp.Server.Settings;
using MyApp.Utilities.Settings;

CustomBootstrapLogger.Create();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

configuration.AddInfrastructureAppsettings()
    .AddEnvironmentVariables();

builder.Services.AddInfrastructureServices<IPresentationAssemblyMarker>(configuration)
    .AddApplicationServices()
    .AddPresentationServices(builder.Environment);

var settings = builder.Services.AddCustomSettings<ServerSettings>(configuration);
builder.WebHost.UseUrls(settings.Urls);

var app = builder.Build();

app.UseCustomInfrastructure()
    .UseCustomPresentation();

await app.SeedDatabase();

await app.RunAsync();

namespace MyApp.Server
{
    public partial class ProgramApi { }
}