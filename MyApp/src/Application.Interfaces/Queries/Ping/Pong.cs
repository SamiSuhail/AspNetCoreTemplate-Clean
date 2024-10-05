namespace MyApp.Application.Interfaces.Queries.Ping;

public record Pong(string Message = Pong.DefaultMessage)
{
    public const string DefaultMessage = nameof(Pong);
};