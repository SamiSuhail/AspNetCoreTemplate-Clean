using Aspire.AppHost.MyApp;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddMyApp();

builder.Build().Run();
