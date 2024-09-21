using MyApp.Infrastructure.Logging;

CustomBootstrapLogger.Create();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddInfrastructureServices(configuration)
    .AddApplicationServices()
    .AddPresentationServices(builder.Environment);

var app = builder.Build();

app.UseCustomInfrastructure()
    .UseCustomPresentation();

await app.RunAsync();

namespace MyApp.Server
{
    public partial class ProgramApi { }
}