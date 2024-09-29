using MyApp.Infrastructure.Logging;
using MyApp.Server;

CustomBootstrapLogger.Create();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

configuration.AddInfrastructureAppsettings()
    .AddEnvironmentVariables()
    .AddInMemoryCollection(ProgramApi.ConfigurationOverrides);

builder.Services.AddInfrastructureServices(configuration)
    .AddApplicationServices()
    .AddPresentationServices(builder.Environment);

var app = builder.Build();

app.UseCustomInfrastructure()
    .UseCustomPresentation();

await app.RunAsync();

namespace MyApp.Server
{
    public partial class ProgramApi 
    {
        // Used only by tests
        public static IEnumerable<KeyValuePair<string, string?>>? ConfigurationOverrides { get; set; } = null;
    }
}