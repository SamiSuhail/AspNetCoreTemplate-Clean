using System.Reflection;
using DbUp.Engine;
using DbUp;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MyApp.DbDeploy;

public static class Helpers
{
    public static string GetConnectionStringFromFile()
    {
        var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        var connectionStrings = configuration.GetSection(nameof(ConnectionStrings))
            .Get<ConnectionStrings>()!;

        var connectionString = connectionStrings.Default;
        return connectionString;
    }

    public static void DropPostgresqlDatabase(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        var databaseName = builder.Database;
        builder.Database = "postgres";

        using var connection = new NpgsqlConnection(builder.ToString());
        connection.Open();

        using (var command = new NpgsqlCommand($"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '{databaseName}\';", connection))
        {
            command.ExecuteNonQuery();
        }

        using (var command = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{databaseName}\";", connection))
        {
            command.ExecuteNonQuery();
        }

        connection.Close();
        Console.WriteLine("Dropped database {0}", databaseName);
    }

    public static DatabaseUpgradeResult DeployDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var upgrader =
            DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(typeof(ProgramDbDeploy).Assembly, path => path.Contains(".Scripts."))
                .LogToConsole()
                .Build();

        return upgrader.PerformUpgrade();
    }
}
