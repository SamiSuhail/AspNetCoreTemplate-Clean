using MyApp.Server.Application;
using MyApp.Server.Infrastructure;
using MyApp.Server.Infrastructure.Logging;
using MyApp.Server.Presentation;
using MyApp.Server.Presentation.Startup;
using static MyApp.Server.Infrastructure.Logging.LoggingStartupExtensions;

CreateCustomSerilogBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddInfrastructureServices(configuration)
    .AddApplicationServices(configuration)
    .AddPresentationServices(builder.Environment);

var app = builder.Build();
app.UseCustomSerilog();
app.UseExceptionHandler(options => { });
app.UseCustomSwagger();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapCustomEndpoints();
app.MapCustomGraphQL();

await app.RunAsync();

namespace MyApp.Server
{
    public partial class ProgramApi { }
}