namespace MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.Register;

public record RegisterRequest(
    string Email,
    string Username,
    string Password);
