using MediatR;
using Npgsql;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.Server.Application.Queries.Ping;

public record PingDatabaseRequest() : IRequest<Pong>;

public class PingDatabaseQueryHandler(ConnectionStringsSettings settings) : IRequestHandler<PingDatabaseRequest, Pong>
{
    public async Task<Pong> Handle(PingDatabaseRequest request, CancellationToken cancellationToken)
    {
        using var connection = new NpgsqlConnection(settings.Database);
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT '{nameof(Pong)}'";
        await connection.OpenAsync(cancellationToken);
        var response = await command.ExecuteScalarAsync(cancellationToken);
        await connection.CloseAsync();
        return new Pong(response!.ToString()!);
    }
}
