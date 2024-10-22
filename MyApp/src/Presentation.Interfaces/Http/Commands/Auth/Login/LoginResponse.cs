namespace MyApp.Presentation.Interfaces.Http.Commands.Auth.Login;

public record LoginResponse(string AccessToken, string RefreshToken);
