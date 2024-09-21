namespace MyApp.Application.Queries.Ping;

public record Pong(string Message = Pong.DefaultMessage)
{
    public const string DefaultMessage = nameof(Pong);
};