using static Aspire.AppHost.MyApp.MyAppConstants;

namespace Aspire.AppHost.MyApp;

public static class MyAppOrchestrationExtensions
{
    public static IResourceBuilder<ProjectResource> AddMyApp(this IDistributedApplicationBuilder builder)
    {
        var db = builder.AddDatabase();
        builder.AddDbDeploy(db);
        var queueServer = builder.AddQueue();

        var api = builder.AddProject<Projects.Server>(Services.Api)
            .WithReference(db, nameof(Services.Database))
            .WithReference(queueServer, nameof(Services.Queue)); // need to add environment variables as well, since project doesn't use connection string

        return api;
    }

    private static IResourceBuilder<PostgresDatabaseResource> AddDatabase(this IDistributedApplicationBuilder builder)
    {
        var dbNameParameter = builder.AddParameter(Database.NameParameter, false);
        var usernameParameter = builder.AddParameter(Database.UsernameParameter, false);
        var passwordParameter = builder.AddParameter(Database.PasswordParameter, true);
        var portParameter = builder.AddParameter(Database.PortParameter, false);

        var dbName = dbNameParameter.Resource.Value;
        var port = int.Parse(portParameter.Resource.Value);

        var server = builder.AddPostgres(Services.DatabaseServer, usernameParameter, passwordParameter, port)
            .WithPgAdmin();

        var db = server.AddDatabase(Services.Database, dbName);
        return db;
    }

    private static void AddDbDeploy(this IDistributedApplicationBuilder builder, IResourceBuilder<PostgresDatabaseResource> db)
    {
        builder.AddProject<Projects.DbDeploy>(Services.DbDeploy)
                    .WithReference(db, nameof(Services.Database));
    }

    private static IResourceBuilder<RabbitMQServerResource> AddQueue(this IDistributedApplicationBuilder builder)
    {
        var usernameParameter = builder.AddParameter(Queue.UsernameParameter, false);
        var passwordParameter = builder.AddParameter(Queue.PasswordParameter, true);
        var portParameter = builder.AddParameter(Queue.PortParameter, false);
        var managementUiPortParameter = builder.AddParameter(Queue.ManagementUiPortParameter, false);

        var port = int.Parse(portParameter.Resource.Value);
        var managementUiPort = int.Parse(managementUiPortParameter.Resource.Value);

        var queueServer = builder.AddRabbitMQ(Services.Queue, usernameParameter, passwordParameter, port)
            .WithManagementPlugin(managementUiPort);
        return queueServer;
    }
}
