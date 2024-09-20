namespace MyApp.Server.Domain.UserManagement.PasswordChangeConfirmation.Failures;

public class PasswordChangeExpiredFailure : DomainFailure
{
    public const string Key = "User";
    public const string Message = "The request for changing password has expired.";

    private PasswordChangeExpiredFailure() { }

    public static DomainException Exception()
        => new PasswordChangeExpiredFailure()
        .AddError(Key, Message)
        .ToException();
}
