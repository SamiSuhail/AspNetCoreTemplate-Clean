using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Presentation.Interfaces.Email;

public static class PasswordResetConstants
{
    public const string SubjectTemplate = "Go2Gether Password Reset - {0}";
    public const string MessageTemplate = """
        Please use the code below to reset your password. This code is valid for {0} minutes after time of requesting it. <br />
        Code: {1}
        """;

    public static string Subject(string username)
        => string.Format(SubjectTemplate, username);
    public static string Message(string code)
        => string.Format(MessageTemplate, BaseConfirmationConstants.ExpirationTimeMinutes, code);
}
