using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Infrastructure.BackgroundJobs;
using MyApp.Server.Infrastructure.Database;
using MyApp.Server.Infrastructure.Email;
using MyApp.Server.Infrastructure.Endpoints;
using MyApp.Server.Infrastructure.ErrorHandling;
using MyApp.Server.Infrastructure.GraphQL;
using MyApp.Server.Infrastructure.Logging;
using MyApp.Server.Infrastructure.MediatR;
using MyApp.Server.Infrastructure.Messaging;
using MyApp.Server.Infrastructure.RequestTransformation;
using MyApp.Server.Infrastructure.Swagger;
using MyApp.Server.Infrastructure.Validation;
using static MyApp.Server.Infrastructure.Logging.StartupExtensions;

CreateCustomSerilogBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddHttpContextAccessor()
    .AddCustomSerilog(configuration)
    .AddCustomAuth(configuration)
    .AddCustomSwagger()
    .AddCustomDatabase(configuration)
    .AddCustomValidators()
    .AddCustomMediatR()
    .AddCustomRequestTransformers()
    .AddCustomEmail(configuration)
    .AddCustomBackgroundJobs(configuration)
    .AddCustomMessaging(configuration)
    .AddCustomGraphQL(builder.Environment)
    .AddExceptionHandler<CustomExceptionHandler>();

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

public partial class ProgramApi { }