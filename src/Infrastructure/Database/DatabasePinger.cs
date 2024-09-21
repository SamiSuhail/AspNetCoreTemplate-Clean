using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Queries.Ping;
using Npgsql;

namespace MyApp.Infrastructure.Database;

public class DatabasePinger(ConnectionStringsSettings settings) : IDatabasePinger
{
    public async Task<string> Ping(CancellationToken cancellationToken)
    {
        using var connection = new NpgsqlConnection(settings.Database);
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT '{Pong.DefaultMessage}'";
        await connection.OpenAsync(cancellationToken);
        var response = await command.ExecuteScalarAsync(cancellationToken);
        await connection.CloseAsync();
        return response?.ToString()
            ?? throw new Exception("Database returned null on ping.");
    }
}
