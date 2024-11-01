namespace MyApp.Presentation.Interfaces.Http.Commands.Auth.Login;

public record LoginRequest(
    string Username,
    string Password,
    string[] Scopes);
