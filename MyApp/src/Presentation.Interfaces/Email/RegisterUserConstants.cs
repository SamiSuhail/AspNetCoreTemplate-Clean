using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Presentation.Interfaces.Email;

public static class RegisterUserConstants
{
    public const string SubjectTemplate = "Go2Gether User Confirmation - {0}";
    public const string MessageTemplate = """
        Please use the code below to confirm your user account. This code is valid for {0} minutes after time of requesting it. <br />
        Code: {1}
        """;

    public static string Subject(string username)
        => string.Format(SubjectTemplate, username);
    public static string Message(string code)
        => string.Format(MessageTemplate, BaseConfirmationConstants.ExpirationTimeMinutes, code);
}
