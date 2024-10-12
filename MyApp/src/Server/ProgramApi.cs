using MyApp.Infrastructure.Logging;

CustomBootstrapLogger.Create();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

configuration.AddInfrastructureAppsettings()
    .AddEnvironmentVariables();

builder.Services.AddInfrastructureServices(configuration)
    .AddApplicationServices()
    .AddPresentationServices(builder.Environment);

var app = builder.Build();

app.UseCustomInfrastructure()
    .UseCustomPresentation();

await app.SeedDatabase();

await app.RunAsync();

namespace MyApp.Server
{
    public partial class ProgramApi { }
}