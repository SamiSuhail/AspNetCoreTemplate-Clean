using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Application.Interfaces.Email;

public static class EmailConstants
{
    public static class SendUserConfirmation
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

    public static class ForgotPassword
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

    public static class ChangePassword
    {
        public const string SubjectTemplate = "Go2gether Password Change - {0}";
        public const string MessageTemplate = """
        Please use the code below to confirm your password change request. This code is valid for {0} minutes after time of requesting it. <br />
        Code: {1}
        """;

        public static string Subject(string username)
            => string.Format(SubjectTemplate, username);
        public static string Message(string code)
            => string.Format(MessageTemplate, BaseConfirmationConstants.ExpirationTimeMinutes, code);
    }

    public static class ChangeEmail
    {
        public const string SubjectTemplate = "Go2Gether Email Change Confirmation - {0}";
        public const string MessageTemplate = """
        Please use the code below to confirm your email change request. This code is valid for {0} minutes after time of requesting it. <br />
        Code: {1}
        """;

        public static string Subject(string username)
            => string.Format(SubjectTemplate, username);
        public static string Message(string code)
            => string.Format(MessageTemplate, BaseConfirmationConstants.ExpirationTimeMinutes, code);
    }
}
