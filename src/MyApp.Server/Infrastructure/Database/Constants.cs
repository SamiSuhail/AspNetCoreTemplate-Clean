namespace MyApp.Server.Infrastructure.Database;

internal static class Constants
{
    internal static class Schemas
    {
        public const string Auth = "auth";
        public const string UserManagement = "user_management";
    }

    internal static class Tables
    {
        public const string Users = "users";
        public const string UserConfirmations = "user_confirmations";
        public const string PasswordResetConfirmations = "password_reset_confirmations";
        public const string EmailChangeConfirmations = "email_change_confirmations";
        public const string PasswordChangeConfirmations = "password_change_confirmations";
    }
}
